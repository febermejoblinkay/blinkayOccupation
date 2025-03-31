using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.ParkingEvent;
using BlinkayOccupation.Application.Services.Stay;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace BlinkayOccupation.API.Controllers;

[ApiController]
[Route("api/devices/parking-events")]
[Authorize(Roles = "admin,user,device")]
public class ParkingEventsController : ControllerBase
{

    private readonly ILogger<ParkingEventsController> _logger;
    private readonly IParkingEventsService _parkingEventsService;

    private IValidator<CreateVehicleParkingRequest> _validator;

    public ParkingEventsController(
        ILogger<ParkingEventsController> logger,
        IParkingEventsService parkingEventsService,
        IValidator<CreateVehicleParkingRequest> validator)
    {
        _logger = logger;
        _parkingEventsService = parkingEventsService;
        _validator = validator;
    }

    [HttpPost("vehicle"), Consumes("multipart/form-data")]
    public async Task<IActionResult> AddVehicle([FromForm, BindRequired] CreateVehicleParkingRequest request, [BindRequired] IEnumerable<IFormFile> files)
    {
        var requestMessage = JsonConvert.SerializeObject(request);
        _logger.LogInformation("BlinkayOccupation-AddVehicle: request received: {request}", requestMessage);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var attachments = GetAttachments(files);
        request.Pictures = attachments;
        var newId = await _parkingEventsService.CreateParkingEvent(request);
        _logger.LogInformation("BlinkayOccupation-AddVehicle: New Parking Event with Id: {id} created correctly.", newId);

        return Ok(newId);
    }

    private static IEnumerable<Application.Models.Blob> GetAttachments(IEnumerable<IFormFile> files)
    {
        return files.Select(file => new Application.Models.Blob(file.ContentType, file.OpenReadStream()));
    }
}
