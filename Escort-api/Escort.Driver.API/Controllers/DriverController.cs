using Escort.Driver.API.DTO;
using Escort.Driver.Application.Repositories;
using Escort.Driver.Infrastructure;

namespace Escort.Driver.API.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DriverController : Controller
{
    private readonly IDriverRepository _driverRepository;
    
    public DriverController(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllDrivers()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return Ok(drivers.Select(driver => driver.ToDto()));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDriverById(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        return Ok(driver.ToDto());
    }
    
    [HttpPost]
    public async Task<ActionResult<IEnumerable<DriverGetDTO>>> CreateDriver(DriverPostPutDto driverPostPutDto)
    {
        var driver = await _driverRepository.CreateAsync(driverPostPutDto.ToDomain());
        return CreatedAtAction(nameof(GetDriverById), new { id = driver.Id }, driver.ToDto());
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DriverGetDTO>> UpdateDriver(Guid id, [FromBody] DriverPostPutDto driverPostPutDto)
    {
        {
            try
            {
                var driver = driverPostPutDto.ToDomain();
                driver.Id = id;
                await _driverRepository.UpdateAsync(driver);
                var driverGetDto = driver.ToDto();

                return Ok(driverGetDto);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }
        }
    }
    
}