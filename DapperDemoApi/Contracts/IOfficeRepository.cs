using DapperDemoApi.Dtos.Offices;
using DapperDemoApi.Entities;

namespace DapperDemoApi.Contracts;

public interface IOfficeRepository
{
    Task<IEnumerable<Office>> ListOfficesAsync();

    Task<Office?> GetOfficeByCodeAsync(string code);

    Task<Office> CreateOfficeAsync(OfficeForCreationDto office);

    Task UpdateOfficeAsync(string code, OfficeForUpdationDto office);

    Task DeleteOfficeAsync(string code);

    Task<IEnumerable<Office>> ListOfficesWithSPAsync();

    Task<Office?> GetOfficeEmployeesMultipleResultsAsync(string code);

    Task<IEnumerable<Office>> GetOfficesEmployeesMultipleMappingAsync();
}
