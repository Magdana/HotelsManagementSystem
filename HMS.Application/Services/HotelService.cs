using HMS.Application.DTOs.Hotel;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Models;
using HMS.Domain.Entities;
using Mapster;
using MapsterMapper;
using System.Net;

namespace HMS.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;

    public HotelService(IHotelRepository hotelRepository, IMapper mapper, IRoomRepository roomRepository, IReservationRepository reservationRepository)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<ApiResponse<HotelResponseDto>> CreateAsync(CreateHotelDto dto)
    {
        ValidateCreateHotelDto(dto);
        var hotel = _mapper.Map<Hotel>(dto);
        try
        {
            await _hotelRepository.AddAsync(hotel);
            await _hotelRepository.SaveAsync();
        }
        catch (Exception)
        {

            throw;
        }

        var hotelResponse = _mapper.Map<HotelResponseDto>(hotel);
        return ApiResponse<HotelResponseDto>.Ok(hotelResponse, "Hotel created successfully", HttpStatusCode.Created, true);

    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        ValidateHotelId(id);

        var hotel = await _hotelRepository.GetAsync(h => h.Id == id);
        if (hotel is null)
            return ApiResponse<bool>.Fail("Hotel not found.");

        var hasRooms = await _roomRepository.ExistsAsync(r => r.HotelId == id);
        if (hasRooms)
            return ApiResponse<bool>.Fail("Cannot delete hotel: it has existing rooms. Remove all rooms first.");

        var hasActiveReservations = await _reservationRepository.ExistsAsync(r =>
            r.Rooms!.Any(rr => rr.HotelId == id) &&
            r.CheckOutDate >= DateTime.UtcNow.Date);
        if (hasActiveReservations)
            return ApiResponse<bool>.Fail("Cannot delete hotel: it has active reservations.");

        try
        {
            _hotelRepository.Remove(hotel);
            await _hotelRepository.SaveAsync();
        }
        catch (Exception)
        {

            throw;
        }
        return ApiResponse<bool>.Ok(true, "Hotel deleted successfully.");

    }

    public async Task<ApiResponse<List<HotelResponseDto>>> GetAllAsync(HotelFilterDto filter)
    {
        var (items, _) = await _hotelRepository.GetAllAsync(
            filter: h =>
                (filter.Country == null || h.Country == filter.Country) &&
                (filter.City == null || h.City == filter.City) &&
                (filter.Rating == null || h.Rating == filter.Rating),
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: "Name",
            ascending: true,
            tracking: false);

        return ApiResponse<List<HotelResponseDto>>.Ok(_mapper.Map<List<HotelResponseDto>>(items));
    }


    public async Task<ApiResponse<HotelResponseDto>> GetByIdAsync(int id)
    {
        ValidateHotelId(id);
        var hotel = await _hotelRepository.GetAsync(h => h.Id == id, tracking: false);
        if (hotel is null)
            return ApiResponse<HotelResponseDto>.Fail("Hotel not found.");
        return ApiResponse<HotelResponseDto>.Ok(_mapper.Map<HotelResponseDto>(hotel));

    }

    public async Task<ApiResponse<HotelResponseDto>> UpdateAsync(int id, UpdateHotelDto dto)
    {
        ValidateHotelId(id);
        ValidateUpdateHotelDto(dto);
        var hotel = await _hotelRepository.GetAsync(h => h.Id == id);
        if (hotel is null)
            return ApiResponse<HotelResponseDto>.Fail("Hotel not found.");

        hotel.Name = dto.Name;
        hotel.Address = dto.Address;
        hotel.Rating = dto.Rating;
        try
        {
            _hotelRepository.Update(hotel);
            await _hotelRepository.SaveAsync();
        }
        catch (Exception)
        {

            throw;
        }
        return ApiResponse<HotelResponseDto>.Ok(_mapper.Map<HotelResponseDto>(hotel), "Hotel updated successfully.");
    }



    #region Validators
    private static void ValidateCreateHotelDto(CreateHotelDto model)
    {
        if (model == null)
            throw new ArgumentException("Request body is required.");

        if (string.IsNullOrWhiteSpace(model.Name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(model.Address))
            throw new ArgumentException("Address is required.");
        if (model.Rating < 1 || model.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(model.Country))
            throw new ArgumentException("Country is required.");
        if (string.IsNullOrWhiteSpace(model.City))
            throw new ArgumentException("City is required.");
    }

    private static void ValidateUpdateHotelDto(UpdateHotelDto model)
    {
        if (model == null)
            throw new ArgumentException("Request body is required.");
        if (string.IsNullOrWhiteSpace(model.Name))
            throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(model.Address))
            throw new ArgumentException("Address is required.");
        if (model.Rating < 1 || model.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
    }

    private static void ValidateHotelId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid hotel ID.");
    }

    #endregion
}


