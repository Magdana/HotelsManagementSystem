using HMS.Application.DTOs.Hotel;
using HMS.Application.Models;
namespace HMS.Application.Interfaces.ServiceInterfaces;

public interface IHotelService
{
    Task<ApiResponse<HotelResponseDto>> CreateAsync(CreateHotelDto dto);
    Task<ApiResponse<HotelResponseDto>> UpdateAsync(int id, UpdateHotelDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<HotelResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<List<HotelResponseDto>>> GetAllAsync(HotelFilterDto filter);
}
