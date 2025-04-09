using BlinkayOccupation.Application.Services.Occupation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkayOccupation.API.Controllers;

[ApiController]
[Route("api/occupations")]
[Authorize(Roles = "admin,user")]
public class OccupationsController : ControllerBase
{

    private readonly ILogger<OccupationsController> _logger;
    private readonly IOccupationsService _occupationsService;

    public OccupationsController(
        ILogger<OccupationsController> logger,
        IOccupationsService occupationsService)
    {
        _logger = logger;
        _occupationsService = occupationsService;
    }

    [HttpGet("current")]
    public async Task<IActionResult> Current()
    {

        var currentOccupations = await _occupationsService.GetCurrentOccupation();
        return Ok(currentOccupations);
    }
}
