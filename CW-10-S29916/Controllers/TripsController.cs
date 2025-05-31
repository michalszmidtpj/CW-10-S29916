using CW_10_S29916.DTOs;
using CW_10_S29916.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_10_S29916.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var pagedResult = await dbService.GetPagedTripsAsync(page, pageSize);
        return Ok(pagedResult);
    }


    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] CreateClientTripDto dto)
    {
        if (!await dbService.TripExistsAsync(idTrip))
            return NotFound($"No such trip");


        if (await dbService.TripHasStartedAsync(idTrip))
            return BadRequest("Already registered");


        if (await dbService.ClientByPeselExistsAsync(dto.Pesel))
            return BadRequest("Already exists");


        if (await dbService.ClientAlreadyOnTripAsync(dto.Pesel, idTrip))
            return BadRequest("Already registered");


        await dbService.AddClientToTripAsync(idTrip, dto);
        return CreatedAtAction(nameof(AddClientToTrip), new { idTrip },
            "Client registered successfully.");
    }
}