using HMS.Application.DTOs.Manager;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/managers")]
[Authorize]
public class ManagersController : ControllerBase
{
    private readonly IManagerService _managerService;

    public ManagersController(IManagerService managerService)
    {
        _managerService = managerService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll(int hotelId)
    {
        var result = await _managerService.GetByHotelIdAsync(hotelId);
        return Ok(result);
    }

    [HttpGet("{managerId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetById(int hotelId, int managerId)
    {
        var result = await _managerService.GetByIdAsync(hotelId, managerId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(int hotelId, [FromBody] RegisterManagerDto dto)
    {
        var result = await _managerService.RegisterAsync(hotelId, dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { hotelId, managerId = result.Result!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{managerId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int hotelId, int managerId, [FromBody] UpdateManagerDto dto)
    {
        if (User.IsInRole("Manager") && !IsManagerOfHotel(hotelId))
            return Forbid();

        var result = await _managerService.UpdateAsync(hotelId, managerId, dto);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{managerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int hotelId, int managerId)
    {
        var result = await _managerService.DeleteAsync(hotelId, managerId);
        if (!result.IsSuccess) return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return NoContent();
    }

    private bool IsManagerOfHotel(int hotelId)
    {
        var claim = User.FindFirst("hotelId")?.Value;
        return int.TryParse(claim, out var id) && id == hotelId;
    }
}
