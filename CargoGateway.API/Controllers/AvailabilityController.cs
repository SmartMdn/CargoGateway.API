using CargoGateway.Core.Interfaces;
using CargoGateway.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CargoGateway.API.Controllers;

[ApiController]
[Route("availability")]
public class AvailabilityController(ICargoService cargoService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SearchAvailability([FromBody] AvailabilitySearchRequest request)
    {
        try
        {
            var result = await cargoService.SearchAsync(request);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new {message = "Failed to search for cargo availability.", details = e.Message});
        }
    }
}