using HMS.Application.DTOs.Manager;
using HMS.Application.Models;

namespace HMS.Application.Interfaces.ServiceInterfaces;

public interface IAdminService
{
    Task<ApiResponse<AdminResponseDto>> RegisterAsync(RegisterAdminDto dto);
    Task<ApiResponse<AdminResponseDto>> UpdateAsync(int adminId, UpdateAdminDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int adminId);
    Task<ApiResponse<List<AdminResponseDto>>> GetAllAsync();
    Task<ApiResponse<AdminResponseDto>> GetByIdAsync(int adminId);
}
