using HMS.Application.DTOs.Hotel;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> Hotels([FromQuery] HotelFilterDto filter)
    {
        var result = await _hotelService.GetAllAsync(filter);
        return Ok(result);
    }



}
