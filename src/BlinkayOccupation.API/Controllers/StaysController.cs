using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.Stay;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("add")]
    [AllowAnonymous]
    public async Task<IActionResult> Add([FromBody] AddStayRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var newId = await _stayService.AddStay(request);

        return Ok(newId);
    }

    [HttpPut("update")]
    [AllowAnonymous]
    public async Task<IActionResult> Update([FromBody] UpdateStayRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var validationResult = await _updatevalidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var updatedId = await _stayService.UpdateStay(request);

        return Ok(updatedId);
    }
}
