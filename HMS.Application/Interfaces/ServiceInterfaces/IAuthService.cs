using HMS.Application.DTOs.Auth;
using HMS.Application.DTOs.Guest;
using HMS.Application.Models;

namespace HMS.Application.Interfaces.ServiceInterfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<ApiResponse<AuthResponseDto>> RegisterGuestAsync(RegisterGuestDto dto);
}
