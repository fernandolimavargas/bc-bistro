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
    
    public async Task<List<VendasHistoricos>> BuscarVendas()
    {
        var sql = @"
            SELECT
                id,
                hora_venda AS DataVenda,

                (
                    SELECT COALESCE(SUM(total), 0)
                    FROM vendas
                ) AS ValorVendidoGeral,

                (
                    SELECT COALESCE(SUM(total), 0)
                    FROM vendas
                    WHERE DATE(hora_venda) = CURRENT_DATE
                ) AS ValorVendidoHoje,

                (
                    SELECT COUNT(*)
                    FROM vendas
                    WHERE DATE(hora_venda) = CURRENT_DATE
                ) AS VendasTotaisHoje

            FROM vendas
            ORDER BY hora_venda DESC;
        ";

        using var connection = CreateConnection();

        var dados = await connection.QueryAsync<VendasHistoricos>(sql);

        return dados.ToList();
    }
}