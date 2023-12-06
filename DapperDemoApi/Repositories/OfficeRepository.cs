using Dapper;
using DapperDemoApi.Contexts;
using DapperDemoApi.Contracts;
using DapperDemoApi.Dtos.Offices;
using DapperDemoApi.Entities;
using System.Data;

namespace DapperDemoApi.Repositories;

public class OfficeRepository : IOfficeRepository
{
    private readonly DapperContext _context;

    public OfficeRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Office>> ListOfficesAsync()
    {
        string query = "SELECT * FROM Offices";

        using (var connection = _context.CreateConnection())
        {
            var offices = await connection.QueryAsync<Office>(query);
            return offices;
        }
    }

    public async Task<Office?> GetOfficeByCodeAsync(string code)
    {
        string query = "SELECT * FROM Offices WHERE officeCode = @code";

        using (var connection = _context.CreateConnection())
        {
            Office? office = await connection.QuerySingleOrDefaultAsync<Office>(query, new { code });
            return office;
        }
    }

    public async Task<Office> CreateOfficeAsync(OfficeForCreationDto office)
    {
        string query = """
            INSERT INTO offices(OfficeCode, City, Phone, AddressLine1, AddressLine2, State, Country, PostalCode, Territory) 
            VALUES 
            (@OfficeCode, @City, @Phone, @AddressLine1, @AddressLine2, @State, @Country, @PostalCode, @Territory);
            """;

        var parameters = new DynamicParameters();
        parameters.Add("OfficeCode", office.OfficeCode, DbType.String);
        parameters.Add("City", office.City, DbType.String);
        parameters.Add("Phone", office.Phone, DbType.String);
        parameters.Add("AddressLine1", office.AddressLine1, DbType.String);
        parameters.Add("AddressLine2", office.AddressLine2, DbType.String);
        parameters.Add("State", office.State, DbType.String);
        parameters.Add("Country", office.Country, DbType.String);
        parameters.Add("PostalCode", office.PostalCode, DbType.String);
        parameters.Add("Territory", office.Territory, DbType.String);

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(query, parameters);

        Office createdOffice = new Office()
        {
            OfficeCode = office.OfficeCode,
            City = office.City,
            Phone = office.Phone,
            AddressLine1 = office.AddressLine1,
            AddressLine2 = office.AddressLine2,
            State = office.State,
            Country = office.Country,
            PostalCode = office.PostalCode,
            Territory = office.Territory
        };

        return createdOffice;
    }

    public async Task UpdateOfficeAsync(string code, OfficeForUpdationDto office)
    {
        string query = """
            UPDATE offices
            SET 
                City = @City, 
                Phone = @Phone, 
                AddressLine1 = @AddressLine1, 
                AddressLine2 = @AddressLine2, 
                State = @State, 
                Country = @Country, 
                PostalCode = @PostalCode, 
                Territory = @Territory
            WHERE
                OfficeCode = @Code;
            """;

        var parameters = new DynamicParameters();
        parameters.Add("Code", code, DbType.String);
        parameters.Add("City", office.City, DbType.String);
        parameters.Add("Phone", office.Phone, DbType.String);
        parameters.Add("AddressLine1", office.AddressLine1, DbType.String);
        parameters.Add("AddressLine2", office.AddressLine2, DbType.String);
        parameters.Add("State", office.State, DbType.String);
        parameters.Add("Country", office.Country, DbType.String);
        parameters.Add("PostalCode", office.PostalCode, DbType.String);
        parameters.Add("Territory", office.Territory, DbType.String);

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(query, parameters);
    }

    public async Task DeleteOfficeAsync(string code)
    {
        string query = """
            DELETE FROM Offices WHERE officeCode = @Code
            """;

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { code });
    }

    public async Task<IEnumerable<Office>> ListOfficesWithSPAsync()
    {
        string procedureName = "ListOffices";
        using var connection = _context.CreateConnection();
        var offices = await connection.QueryAsync<Office>(procedureName, commandType: CommandType.StoredProcedure);
        return offices;
    }

    public async Task<Office?> GetOfficeEmployeesMultipleResultsAsync(string code)
    {
        string queries = """
            SELECT * FROM offices WHERE officeCode = @code;
            SELECT * FROM employees WHERE officeCode = @code;
            """;
        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(queries, new { code });
        Office? office = await multi.ReadSingleOrDefaultAsync<Office>();
        if (office is not null)
        {
            office.Employees = (await multi.ReadAsync<Employee>()).ToList();
        }
        return office;
    }

    public async Task<IEnumerable<Office>> GetOfficesEmployeesMultipleMappingAsync()
    {
        string query = """
            SELECT *
            FROM Offices o 
            INNER JOIN Employees e 
                ON o.OfficeCode = e.OfficeCode;
            """;

        using var connection = _context.CreateConnection();
        var officeDict = new Dictionary<string, Office>();

        var offices = await connection.QueryAsync<Office, Employee, Office>(
            query, (o, e) =>
            {
                if (!officeDict.TryGetValue(o.OfficeCode, out var currentOffice))
                {
                    currentOffice = o;
                    officeDict.Add(currentOffice.OfficeCode, currentOffice);
                }

                if (currentOffice.Employees == null)
                {
                    currentOffice.Employees = new List<Employee>();
                }
                currentOffice.Employees.Add(e);
                return currentOffice;
            }, splitOn: "EmployeeNumber");

        return offices.Distinct().ToList();
    }
}