using CargoGateway.API.Models;
using CargoGateway.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoGateway.API.Controllers;

[ApiController]
[Route("availability")]
public class AvailabilityController : ControllerBase
{
    private readonly CargoService _cargoService;
    
    public AvailabilityController(CargoService cargoService)
    {
        _cargoService = cargoService;
    }

    [HttpPost]
    public async Task<IActionResult> SearchAvailability([FromBody] AvailabilitySearchRequest request)
    {
        try
        {
            var result = await _cargoService.SearchAsync(request);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new {message = "Failed to search for cargo availability.", details = e.Message});
        }
    }
}