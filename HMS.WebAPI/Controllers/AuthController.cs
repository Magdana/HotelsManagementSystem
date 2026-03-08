using HMS.Application.DTOs.Auth;
using HMS.Application.DTOs.Guest;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterGuestDto dto)
    {
        var result = await _authService.RegisterGuestAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
