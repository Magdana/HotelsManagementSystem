using HMS.Application.DTOs.Room;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int hotelId, [FromQuery] RoomFilterDto filter)
    {
        var result = await _roomService.GetAllAsync(hotelId, filter);
        return Ok(result);
    }

    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetById(int hotelId, int roomId)
    {
        var result = await _roomService.GetByIdAsync(hotelId, roomId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    //[Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(int hotelId, [FromBody] CreateRoomDto dto)
    {
        //if (User.IsInRole("Manager") && !IsManagerOfHotel(hotelId))
        //    return Forbid();

        var result = await _roomService.CreateAsync(hotelId, dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { hotelId, roomId = result.Result!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{roomId}")]
    //[Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int hotelId, int roomId, [FromBody] UpdateRoomDto dto)
    {
        //if (User.IsInRole("Manager") && !IsManagerOfHotel(hotelId))
        //    return Forbid();

        var result = await _roomService.UpdateAsync(hotelId, roomId, dto);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{roomId}")]
    //[Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int hotelId, int roomId)
    {
        //if (User.IsInRole("Manager") && !IsManagerOfHotel(hotelId))
        //    return Forbid();

        var result = await _roomService.DeleteAsync(hotelId, roomId);
        if (!result.IsSuccess) return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return NoContent();
    }

    //private bool IsManagerOfHotel(int hotelId)
    //{
    //    var claim = User.FindFirst("hotelId")?.Value;
    //    return int.TryParse(claim, out var id) && id == hotelId;
    //}
}
