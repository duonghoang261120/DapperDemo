namespace DapperDemoApi.Dtos.Offices;

#pragma warning disable

public record OfficeForCreationDto
{
    public string OfficeCode { get; init; }
    public string City { get; init; }
    public string Phone { get; init; }
    public string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? State { get; init; }
    public string Country { get; init; }
    public string PostalCode { get; init; }
    public string Territory { get; init; }
}
