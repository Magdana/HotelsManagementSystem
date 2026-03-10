using HMS.Application.DTOs.Manager;
using HMS.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.WebAPI.Controllers;

[ApiController]
[Route("api/admins")]
[Authorize(Roles = "Admin")]
public class AdminsController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _adminService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{adminId}")]
    public async Task<IActionResult> GetById(int adminId)
    {
        var result = await _adminService.GetByIdAsync(adminId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterAdminDto dto)
    {
        var result = await _adminService.RegisterAsync(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { adminId = result.Result!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{adminId}")]
    public async Task<IActionResult> Update(int adminId, [FromBody] UpdateAdminDto dto)
    {
        var result = await _adminService.UpdateAsync(adminId, dto);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{adminId}")]
    public async Task<IActionResult> Delete(int adminId)
    {
        var result = await _adminService.DeleteAsync(adminId);
        if (!result.IsSuccess) return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return NoContent();
    }
}
