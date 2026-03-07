using HMS.Application.DTOs.Reservation;
using HMS.Application.Models;

namespace HMS.Application.Interfaces.ServiceInterfaces;

public interface IReservationService
{
    Task<ApiResponse<ReservationResponseDto>> CreateAsync(int hotelId, CreateReservationDto dto);
    Task<ApiResponse<ReservationResponseDto>> UpdateAsync(int reservationId, UpdateReservationDto dto);
    Task<ApiResponse<bool>> CancelAsync(int reservationId);
    Task<ApiResponse<ReservationResponseDto>> GetByIdAsync(int reservationId);
    Task<ApiResponse<List<ReservationResponseDto>>> GetAllAsync(ReservationFilterDto filter);

}
