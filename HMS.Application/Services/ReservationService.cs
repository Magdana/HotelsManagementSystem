using HMS.Application.DTOs.Reservation;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Models;
using HMS.Domain.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services;

public class ReservationService: IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ReservationResponseDto>> CreateAsync(int hotelId, CreateReservationDto dto)
    {
        ValidateCreateReservationDto(dto);
        if (hotelId <= 0)
            return ApiResponse<ReservationResponseDto>.Fail("Invalid hotel ID.");
        

        foreach (var roomId in dto.RoomIds)
        {
            var roomExists = await _roomRepository.ExistsAsync(r => r.Id == roomId && r.HotelId == hotelId);
            if (!roomExists)
                return ApiResponse<ReservationResponseDto>.Fail($"Room {roomId} was not found in hotel {hotelId}.");
        }

        var hasConflict = await _reservationRepository.ExistsAsync(r =>
            r.CheckInDate < dto.CheckOutDate &&
            r.CheckOutDate > dto.CheckInDate &&
            r.ReservationRooms!.Any(rr => dto.RoomIds.Contains(rr.RoomId)));
        if (hasConflict)
            return ApiResponse<ReservationResponseDto>.Fail("One or more rooms are not available for the selected dates.");

        var reservation = new Reservation
        {
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            GuestId = dto.GuestId,
            ReservationRooms = dto.RoomIds.Select(id => new ReservationRoom { RoomId = id }).ToList()
        };

        try
        {
            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveAsync();

        }
        catch (Exception)
        {

            throw;
        }
        var created = await _reservationRepository.GetAsync(
            r => r.Id == reservation.Id,
            includes: q => q.Include(r => r.ReservationRooms!).ThenInclude(rr => rr.Room),
            tracking: false);

        return ApiResponse<ReservationResponseDto>.Ok(MapToDto(created!), "Reservation created successfully.");
    }

    

    public async Task<ApiResponse<ReservationResponseDto>> UpdateAsync(int reservationId, UpdateReservationDto dto)
    {
        ValidateUpdateReservationDto(dto);

        var reservation = await _reservationRepository.GetAsync(
            r => r.Id == reservationId,
            includes: q => q.Include(r => r.ReservationRooms!));
        if (reservation is null)
            return ApiResponse<ReservationResponseDto>.Fail("Reservation not found.");

        var roomIds = reservation.ReservationRooms!.Select(rr => rr.RoomId).ToList();

        var hasConflict = await _reservationRepository.ExistsAsync(r =>
            r.Id != reservationId &&
            r.CheckInDate < dto.CheckOutDate &&
            r.CheckOutDate > dto.CheckInDate &&
            r.ReservationRooms!.Any(rr => roomIds.Contains(rr.RoomId)));
        if (hasConflict)
            return ApiResponse<ReservationResponseDto>.Fail("The new dates conflict with an existing reservation.");

        reservation.CheckInDate = dto.CheckInDate;
        reservation.CheckOutDate = dto.CheckOutDate;
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveAsync();

        var updated = await _reservationRepository.GetAsync(
            r => r.Id == reservationId,
            includes: q => q.Include(r => r.ReservationRooms!).ThenInclude(rr => rr.Room),
            tracking: false);

        return ApiResponse<ReservationResponseDto>.Ok(MapToDto(updated!), "Reservation updated successfully.");
    }

    

    public async Task<ApiResponse<bool>> CancelAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetAsync(r => r.Id == reservationId);
        if (reservation is null)
            return ApiResponse<bool>.Fail("Reservation not found.");

        _reservationRepository.Remove(reservation);
        await _reservationRepository.SaveAsync();
        return ApiResponse<bool>.Ok(true, "Reservation cancelled successfully.");
    }

    public async Task<ApiResponse<ReservationResponseDto>> GetByIdAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetAsync(
            r => r.Id == reservationId,
            includes: q => q.Include(r => r.ReservationRooms!).ThenInclude(rr => rr.Room),
            tracking: false);
        if (reservation is null)
            return ApiResponse<ReservationResponseDto>.Fail("Reservation not found.");
        return ApiResponse<ReservationResponseDto>.Ok(MapToDto(reservation));
    }

    public async Task<ApiResponse<List<ReservationResponseDto>>> GetAllAsync(ReservationFilterDto filter)
    {
        var (items, _) = await _reservationRepository.GetAllAsync(
            filter: r =>
                (filter.GuestId == null || r.GuestId == filter.GuestId) &&
                (filter.RoomId == null || r.ReservationRooms!.Any(rr => rr.RoomId == filter.RoomId)) &&
                (filter.HotelId == null || r.ReservationRooms!.Any(rr => rr.Room.HotelId == filter.HotelId)) &&
                (filter.CheckInDate == null || r.CheckInDate >= filter.CheckInDate) &&
                (filter.CheckOutDate == null || r.CheckOutDate <= filter.CheckOutDate) &&
                (filter.Active == null || (filter.Active.Value
                    ? r.CheckOutDate >= DateTime.UtcNow.Date
                    : r.CheckOutDate < DateTime.UtcNow.Date)),
            includes: q => q.Include(r => r.ReservationRooms!).ThenInclude(rr => rr.Room),
            tracking: false);

        return ApiResponse<List<ReservationResponseDto>>.Ok(items.Select(MapToDto).ToList());
    }

    private static ReservationResponseDto MapToDto(Reservation r) => new()
    {
        Id = r.Id,
        CheckInDate = r.CheckInDate,
        CheckOutDate = r.CheckOutDate,
        GuestId = r.GuestId,
        Rooms = r.ReservationRooms?.Select(rr => new RoomSummaryDto
        {
            Id = rr.Room.Id,
            Name = rr.Room.Name,
            Price = rr.Room.Price
        }).ToList() ?? new()
    };


    #region Validation
    
    private void ValidateCreateReservationDto(CreateReservationDto dto)
    {
        if (dto.CheckInDate >= dto.CheckOutDate)
            throw new ArgumentException("Check-in date must be before check-out date.");
        if (dto.RoomIds == null || !dto.RoomIds.Any())
            throw new ArgumentException("At least one room must be selected.");

    }

    private void ValidateUpdateReservationDto(UpdateReservationDto dto)
    {
        if (dto.CheckInDate >= dto.CheckOutDate)
            throw new ArgumentException("Check-in date must be before check-out date.");
        if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Check-in date must be today or in the future.");

    }

    #endregion

}
