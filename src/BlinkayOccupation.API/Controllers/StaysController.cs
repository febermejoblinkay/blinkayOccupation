using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.Stay;
using BlinkayOccupation.Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlinkayOccupation.API.Controllers;

[ApiController]
[Route("api/stays")]
[Authorize(Roles = "admin,user")]
public class StaysController : ControllerBase
{

    private readonly ILogger<StaysController> _logger;
    private readonly IStayService _stayService;

    private IValidator<AddStayRequest> _validator;
    private IValidator<UpdateStayRequest> _updatevalidator;

    public StaysController(
        ILogger<StaysController> logger,
        IStayService stayService,
        IValidator<AddStayRequest> validator,
        IValidator<UpdateStayRequest> updatevalidator)
    {
        _logger = logger;
        _stayService = stayService;
        _validator = validator;
        _updatevalidator = updatevalidator;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddStayRequest request)
    {
        try
        {
            var requestMessage = JsonConvert.SerializeObject(request);
            _logger.LogInformation("BlinkayOccupation-Add: request received: {request}", requestMessage);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationResult = await _validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var newId = await _stayService.AddStay(request);
            _logger.LogInformation("BlinkayOccupation-Add: New stay with Id: {id} created correctly.", newId);

            return Ok(newId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BlinkayOccupation-Add:Something wrong happened when trying to insert an stay.");
            return Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing your request.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStayRequest request)
    {
        try
        {
            var requestMessage = JsonConvert.SerializeObject(request);
            _logger.LogInformation("BlinkayOccupation-Update: request received: {request}", requestMessage);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationResult = await _updatevalidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var updatedId = await _stayService.UpdateStay(request);
            _logger.LogInformation("BlinkayOccupation-Update: Stay with Id: {id} updated correctly.", updatedId);

            return Ok(updatedId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BlinkayOccupation-Update:Something wrong happened when trying to update an stay.");
            return Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing your request.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
}
