using System.IdentityModel.Tokens.Jwt;
using HMS.Application.DTOs.Reservation;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll(int hotelId, [FromQuery] ReservationFilterDto filter)
    {
        filter.HotelId = hotelId;
        var result = await _reservationService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{reservationId}")]
    public async Task<IActionResult> GetById(int hotelId, int reservationId)
    {
        var result = await _reservationService.GetByIdAsync(reservationId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Guest")]
    public async Task<IActionResult> Create(int hotelId, [FromBody] CreateReservationDto dto)
    {
        dto.GuestId = GetCurrentUserId();
        var result = await _reservationService.CreateAsync(hotelId, dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { hotelId, reservationId = result.Result!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{reservationId}")]
    public async Task<IActionResult> Update(int hotelId, int reservationId, [FromBody] UpdateReservationDto dto)
    {
        var result = await _reservationService.UpdateAsync(reservationId, dto);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{reservationId}")]
    public async Task<IActionResult> Cancel(int hotelId, int reservationId)
    {
        var result = await _reservationService.CancelAsync(reservationId);
        if (!result.IsSuccess) return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.TryParse(sub, out var id) ? id : 0;
    }
}
