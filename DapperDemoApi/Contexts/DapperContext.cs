using MySqlConnector;
using System.Data;

namespace DapperDemoApi.Contexts;

public class DapperContext
{
    private const string DEFAULT_CONNECTION_STRING = "";
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("Default");
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}
