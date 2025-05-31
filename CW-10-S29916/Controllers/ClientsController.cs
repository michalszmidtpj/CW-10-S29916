using CW_10_S29916.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_10_S29916.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        if (await dbService.ClientHasAnyTripsAsync(idClient))
            return BadRequest("Cannot delete client");
        
        if (!await dbService.DeleteClientAsync(idClient))
            return NotFound();
        
        return NoContent();
    }
}