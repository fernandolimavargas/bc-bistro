using Npgsql;
using System.Data;

public class ConexaoDapper
{
    private readonly IConfiguration _configuration;

    public ConexaoDapper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(
            _configuration.GetConnectionString("DefaultConnection")
        );
    }
}