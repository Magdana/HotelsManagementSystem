using HMS.Application.DTOs.Hotel;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] HotelFilterDto filter)
    {
        var result = await _hotelService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetById(int hotelId)
    {
        var result = await _hotelService.GetByIdAsync(hotelId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateHotelDto dto)
    {
        var result = await _hotelService.CreateAsync(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { hotelId = result.Result!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{hotelId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int hotelId, [FromBody] UpdateHotelDto dto)
    {
        if (User.IsInRole("Manager") && !IsManagerOfHotel(hotelId))
            return Forbid();

        var result = await _hotelService.UpdateAsync(hotelId, dto);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{hotelId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int hotelId)
    {
        var result = await _hotelService.DeleteAsync(hotelId);
        if (!result.IsSuccess) return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return NoContent();
    }

    private bool IsManagerOfHotel(int hotelId)
    {
        var claim = User.FindFirst("hotelId")?.Value;
        return int.TryParse(claim, out var id) && id == hotelId;
    }
}
