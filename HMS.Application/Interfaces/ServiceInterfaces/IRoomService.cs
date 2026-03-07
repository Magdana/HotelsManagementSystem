using HMS.Application.DTOs.Room;
using HMS.Application.Models;

namespace HMS.Application.Interfaces.ServiceInterfaces;

public interface IRoomService
{
    Task<ApiResponse<RoomResponseDto>> CreateAsync(int hotelId, CreateRoomDto dto);
    Task<ApiResponse<RoomResponseDto>> UpdateAsync(int hotelId, int roomId, UpdateRoomDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int hotelId, int roomId);
    Task<ApiResponse<RoomResponseDto>> GetByIdAsync(int hotelId, int roomId);
    Task<ApiResponse<List<RoomResponseDto>>> GetAllAsync(int hotelId, RoomFilterDto filter);
}
