using System.ComponentModel.Design;
using Dapper;

public class CatalogoRepository : ConexaoDapper
{
    public CatalogoRepository(IConfiguration configuration) : base(configuration) { }

    public List<Catalogo> MostrarCatalogo(int filtro = 0)
    {
        var where = "";
        var parameters = new DynamicParameters();

        if (filtro > 0)
        {
            where += "WHERE p.Categoria = @Categoria";
            parameters.Add("Categoria", filtro);
        }
        

        var sqlProdutos = $@" 
            SELECT 
                p.ID, 
                Nome as Produto, 
                Preco, 
                p.Categoria as IdCategoria,
                ic.categoria as Categoria,
                p.Disponivel
                FROM produtos p 
                INNER JOIN inf_categorias ic
                    on ic.id = p.categoria
                {where}
                ORDER BY p.id";

        var connection = CreateConnection();
        return connection.Query<Catalogo>(sqlProdutos, parameters).ToList();
    }
}