using HMS.Application.DTOs.Manager;
using HMS.Application.Models;

namespace HMS.Application.Interfaces.ServiceInterfaces;

public interface IManagerService
{
    Task<ApiResponse<ManagerResponseDto>> RegisterAsync(int hotelId, RegisterManagerDto dto);
    Task<ApiResponse<ManagerResponseDto>> UpdateAsync(int hotelId, int managerId, UpdateManagerDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int hotelId, int managerId);
    Task<ApiResponse<List<ManagerResponseDto>>> GetByHotelIdAsync(int hotelId);
    Task<ApiResponse<ManagerResponseDto>> GetByIdAsync(int hotelId, int managerId);
}
