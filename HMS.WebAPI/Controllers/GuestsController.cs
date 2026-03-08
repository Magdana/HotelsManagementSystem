using System.IdentityModel.Tokens.Jwt;
using HMS.Application.DTOs.Guest;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[ApiController]
[Route("api/guests")]
[Authorize]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;

    public GuestsController(IGuestService guestService)
    {
        _guestService = guestService;
    }

    [HttpGet("{guestId}")]
    public async Task<IActionResult> GetById(int guestId)
    {
        var result = await _guestService.GetByIdAsync(guestId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPut("{guestId}")]
    public async Task<IActionResult> Update(int guestId, [FromBody] UpdateGuestDto dto)
    {
        if (!User.IsInRole("Admin") && GetCurrentUserId() != guestId)
            return Forbid();

        var result = await _guestService.UpdateAsync(guestId, dto);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{guestId}")]
    public async Task<IActionResult> Delete(int guestId)
    {
        if (!User.IsInRole("Admin") && GetCurrentUserId() != guestId)
            return Forbid();

        var result = await _guestService.DeleteAsync(guestId);
        if (!result.IsSuccess) return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.TryParse(sub, out var id) ? id : 0;
    }
}
