using Dapper;

public class ProdutoRepository: ConexaoDapper
{
    public ProdutoRepository(IConfiguration configuration) : base(configuration) { }

    public List<Produtos> BuscarProdutos()
    {
        var sqlProdutos = @" 
            SELECT 
                p.ID, 
                Nome as Produto, 
                Preco, 
                p.Categoria as IdCategoria,
                ic.categoria as Categoria
                FROM produtos p 
                INNER JOIN inf_categorias ic
                    on ic.id = p.categoria";

        var connection = CreateConnection();
        return connection.Query<Produtos>(sqlProdutos).ToList();
    }

    public async Task AdicionarProduto(Produtos produto)
    {
        var parameters = new
        {
            Produto = produto.Produto,
            Preco = produto.Preco,
            Categoria = produto.IdCategoria

        };
        var sqlProduto = @"INSERT INTO produtos (nome, preco, categoria) 
            VALUES (@Produto, @Preco, @Categoria)";

        var connection = CreateConnection();
        connection.Execute(sqlProduto, parameters);
    }
    
    public async Task AlterarProduto(Produtos produto)
    {
        var parameters = new
        {
            IdProduto = produto.Id,
            Produto = produto.Produto,
            Preco = produto.Preco,
            Categoria = produto.IdCategoria
        };

        var sqlAlterarProduto = @"UPDATE produtos SET
                            nome = @Produto, 
                            preco = @Preco,
                            categoria = @Categoria
                            where id = @IdProduto";

        var connection = CreateConnection();
        connection.Execute(sqlAlterarProduto, parameters);
    }
}