using HMS.Application.DTOs.Guest;
using HMS.Application.Models;

namespace HMS.Application.Interfaces.ServiceInterfaces;

public interface IGuestService
{
    Task<ApiResponse<GuestResponseDto>> RegisterAsync(RegisterGuestDto dto);
    Task<ApiResponse<GuestResponseDto>> UpdateAsync(int id, UpdateGuestDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<GuestResponseDto>> GetByIdAsync(int id);
}
