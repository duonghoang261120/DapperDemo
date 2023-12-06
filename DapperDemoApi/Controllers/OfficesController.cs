using DapperDemoApi.Contracts;
using DapperDemoApi.Dtos.Offices;
using DapperDemoApi.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DapperDemoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfficesController : ControllerBase
{
    private const string OfficeByCodeName = "OfficeByCode";
    private readonly IOfficeRepository _officeRepository;

    public OfficesController(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> ListOffice()
    {
        try
        {
            var offices = await _officeRepository.ListOfficesAsync();
            return Ok(offices);
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{code}", Name = OfficeByCodeName)]
    public async Task<IActionResult> GetOffice(string code)
    {
        try
        {
            Office? office = await _officeRepository.GetOfficeByCodeAsync(code);
            if (office is null)
            {
                return StatusCode(404, "The office was not found.");
            }

            return Ok(office);
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOffice([FromBody] OfficeForCreationDto office)
    {
        try
        {
            Office createdOffice = await _officeRepository.CreateOfficeAsync(office);
            return CreatedAtRoute(OfficeByCodeName, new { code = createdOffice.OfficeCode }, createdOffice);
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOffice(string code, [FromBody] OfficeForUpdationDto office)
    {
        try
        {
            var dbOffice = await _officeRepository.GetOfficeByCodeAsync(code);
            if (dbOffice is null)
            {
                return NotFound("The office was not found.");
            }

            await _officeRepository.UpdateOfficeAsync(code, office);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOffice(string code)
    {
        try
        {
            var dbOffice = await _officeRepository.GetOfficeByCodeAsync(code);
            if (dbOffice is null)
            {
                return NotFound("The office was not found.");
            }

            await _officeRepository.DeleteOfficeAsync(code);
            return NoContent();
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("SP")]
    public async Task<IActionResult> ListOfficesWithSP()
    {
        try
        {
            var offices = await _officeRepository.ListOfficesWithSPAsync();
            return Ok(offices);
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{code}/multiple-result")]
    public async Task<IActionResult> GetOfficeEmployeesMultipleResults(string code)
    {
        try
        {
            Office? officeFromDb = await _officeRepository.GetOfficeEmployeesMultipleResultsAsync(code);
            if (officeFromDb is null) return NotFound("The office was not found.");
            return Ok(officeFromDb);
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("multiple-mapping")]
    public async Task<IActionResult> GetOfficesEmployeesMultipleMapping()
    {
        try
        {
            var companies = await _officeRepository.GetOfficesEmployeesMultipleMappingAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, ex.Message);
        }
    }
}
