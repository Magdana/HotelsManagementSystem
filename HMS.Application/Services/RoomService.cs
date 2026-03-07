using HMS.Application.DTOs.Hotel;
using HMS.Application.DTOs.Room;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Models;
using HMS.Domain.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;

    public RoomService(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        IReservationRepository reservationRepository,
        IMapper mapper)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _reservationRepository = reservationRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RoomResponseDto>> CreateAsync(int hotelId, CreateRoomDto dto)
    {
        ValidateCreateRoomDto(dto);
        if (hotelId <= 0)
            return ApiResponse<RoomResponseDto>.Fail("Invalid hotel ID.");
        var hotelExists = await _hotelRepository.ExistsAsync(h => h.Id == hotelId);
        if (!hotelExists)
            return ApiResponse<RoomResponseDto>.Fail("Hotel not found.");

        var room = _mapper.Map<Room>(dto);
        room.HotelId = hotelId; try
        {

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveAsync();
        }
        catch (Exception)
        {

            throw;
        }
        return ApiResponse<RoomResponseDto>.Ok(_mapper.Map<RoomResponseDto>(room), "Room created successfully.");
    }

    public async Task<ApiResponse<RoomResponseDto>> UpdateAsync(int hotelId, int roomId, UpdateRoomDto dto)
    {
        ValidateUpdateRoomDto(dto);
        if (hotelId <= 0)
            return ApiResponse<RoomResponseDto>.Fail("Invalid hotel ID.");
        if (roomId <= 0)
            return ApiResponse<RoomResponseDto>.Fail("Invalid room ID.");

        var room = await _roomRepository.GetAsync(r => r.Id == roomId && r.HotelId == hotelId);
        if (room is null)
            return ApiResponse<RoomResponseDto>.Fail("Room not found.");

        room.Name = dto.Name;
        room.Price = dto.Price;
        try
        {
            _roomRepository.Update(room);
            await _roomRepository.SaveAsync();
        }
        catch (Exception)
        {

            throw;
        }
        return ApiResponse<RoomResponseDto>.Ok(_mapper.Map<RoomResponseDto>(room), "Room updated successfully.");
    }

    

    public async Task<ApiResponse<bool>> DeleteAsync(int hotelId, int roomId)
    {
        if (hotelId <= 0)
            return ApiResponse<bool>.Fail("Invalid hotel ID.");
        if (roomId <= 0)
            return ApiResponse<bool>.Fail("Invalid room ID.");

        var room = await _roomRepository.GetAsync(r => r.Id == roomId && r.HotelId == hotelId);
        if (room is null)
            return ApiResponse<bool>.Fail("Room not found.");

        var hasActiveReservations = await _reservationRepository.ExistsAsync(r =>
            r.Rooms!.Any(rr => rr.Id == roomId) &&
            r.CheckOutDate >= DateTime.UtcNow.Date);
        if (hasActiveReservations)
            return ApiResponse<bool>.Fail("Cannot delete room: it has active or upcoming reservations.");

        try
        {
            _roomRepository.Remove(room);
            await _roomRepository.SaveAsync();
        }
        catch (Exception)
        {

            throw;
        }
        return ApiResponse<bool>.Ok(true, "Room deleted successfully.");
    }

    public async Task<ApiResponse<RoomResponseDto>> GetByIdAsync(int hotelId, int roomId)
    {
        if (hotelId <= 0)
            return ApiResponse<RoomResponseDto>.Fail("Invalid hotel ID.");
        if (roomId <= 0)
            return ApiResponse<RoomResponseDto>.Fail("Invalid room ID.");
        var room = await _roomRepository.GetAsync(r => r.Id == roomId && r.HotelId == hotelId, tracking: false);
        if (room is null)
            return ApiResponse<RoomResponseDto>.Fail("Room not found.");
        return ApiResponse<RoomResponseDto>.Ok(_mapper.Map<RoomResponseDto>(room));
    }

    public async Task<ApiResponse<List<RoomResponseDto>>> GetAllAsync(int hotelId, RoomFilterDto filter)
    {
        var bookedRoomIds = new List<int>();
        if (filter.CheckInDate.HasValue && filter.CheckOutDate.HasValue)
        {
            var conflicting = await _reservationRepository.GetAllAsync(
                filter: r => r.CheckInDate < filter.CheckOutDate && r.CheckOutDate > filter.CheckInDate,
                includes: q => q.Include(r => r.Rooms!),
                tracking: false);
            bookedRoomIds = conflicting.Items
                .SelectMany(r => r.Rooms!.Select(room => room.Id))
                .Distinct()
                .ToList();
        }

        var (items, _) = await _roomRepository.GetAllAsync(
            filter: r => r.HotelId == hotelId &&
                (filter.MinPrice == null || r.Price >= filter.MinPrice) &&
                (filter.MaxPrice == null || r.Price <= filter.MaxPrice),
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            tracking: false);

        if (bookedRoomIds.Count > 0)
            items = items.Where(r => !bookedRoomIds.Contains(r.Id)).ToList();

        return ApiResponse<List<RoomResponseDto>>.Ok(_mapper.Map<List<RoomResponseDto>>(items));
    }


    #region Validations


    private static void ValidateCreateRoomDto(CreateRoomDto model)
    {
        if (model == null)
            throw new ArgumentException("Request body is required.");

        if (string.IsNullOrWhiteSpace(model.Name))
            throw new ArgumentException("Name is required.");
        if (model.Price <= 0)
            throw new ArgumentException("Price must be greater than 0.");
    }


    private void ValidateUpdateRoomDto(UpdateRoomDto model)
    {
        if (model == null)
            throw new ArgumentException("Request body is required.");
        if (string.IsNullOrWhiteSpace(model.Name))
            throw new ArgumentException("Name is required.");
        if (model.Price <= 0)
            throw new ArgumentException("Price must be greater than 0.");
    }
    #endregion
}
