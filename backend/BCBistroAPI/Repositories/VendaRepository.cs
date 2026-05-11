using Dapper;

public class VendaRepository : ConexaoDapper
{
    public VendaRepository(IConfiguration configuration) : base(configuration) { }

    public async Task FinalizarVenda(Venda venda)
    {
        var parameters = new
        {
            HoraVenda = venda.HoraVenda,
            Total = venda.Total
        };
        var sqlVendas = @"
        INSERT INTO vendas (hora_venda, total) VALUES (@HoraVenda, @Total) RETURNING id";

        var sqlComanda = @"INSERT INTO comandas (produto, quantidade, valor_unidade, valor_calculado, total, id_venda)
                            VALUES(@Produto, @Quantidade, @ValorUnidade, @ValorCalculado, @TotalVenda, @IdVenda)";

        using var connection = CreateConnection();
        connection.Open();
        var transaction = connection.BeginTransaction();
        try
        {

            var idVenda = await connection.ExecuteScalarAsync<int>(sqlVendas, parameters, transaction: transaction);

            foreach (var produto in venda.Produtos)
            {
                var parametersComanda = new
                {
                    Produto = produto.Produto,
                    Quantidade = produto.Quantidade,
                    ValorUnidade = produto.ValorUnidade,
                    ValorCalculado = produto.ValorCalculado,
                    TotalVenda = produto.ValorTotal,
                    IdVenda = idVenda
                };

                await connection.ExecuteAsync(sqlComanda, parametersComanda, transaction: transaction);
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    
    public async Task<List<VendasHistoricos>> BuscarVendas(DateTime dataInicial, DateTime dataFinal)
    {
        var sql = @"
            WITH vendas_filtradas AS (
                SELECT *
                FROM vendas
                WHERE CAST(hora_venda AS DATE)
                    BETWEEN @dataInicial AND @dataFinal
            ),

            vendas_com_total AS (
                SELECT 
                    id,
                    hora_venda,
                    total,
                    SUM(total) OVER (
                        PARTITION BY CAST(hora_venda AS DATE)
                    ) AS totalDoDia
                FROM vendas_filtradas
            )

            SELECT 
                v.id,
                c.produto,
                ic.categoria,
                c.quantidade,
                c.valor_unidade as valorUnidade,
                c.valor_calculado as valorCalculado,
                v.hora_venda as horaVenda,
                v.total as TotalVenda,
                v.totalDoDia
            FROM vendas_com_total v
            INNER JOIN comandas c 
                ON c.id_venda = v.id
            INNER JOIN produtos p 
                ON p.nome = c.produto
            INNER JOIN inf_categorias ic
                ON ic.id = p.categoria
            ORDER BY v.hora_venda;";

        using var connection = CreateConnection();

        var dados = await connection.QueryAsync<VendasHistoricosDTO>(
            sql,
            new
            {
                dataInicial = dataInicial.Date,
                dataFinal = dataFinal.Date
            });

        return dados
            .GroupBy(g => g.Id)
            .Select(s => new VendasHistoricos
            {
                Id = s.Key,
                HoraVenda = s.First().HoraVenda,
                TotalVenda = s.First().TotalVenda,
                TotalDoDia = s.First().TotalDoDia,

                ProdutosVendidos = s.Select(p => new ProdutosVendidos
                {
                    Produto = p.Produto,
                    Categoria = p.Categoria,
                    ValorUnidade = p.ValorUnidade,
                    Quantidade = p.Quantidade,
                    ValorCalculado = p.ValorCalculado
                }).ToList()
            })
            .ToList();
    }
}