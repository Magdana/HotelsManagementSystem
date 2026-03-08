using HMS.Application.DTOs.Guest;
using HMS.Application.Interfaces;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Models;
using Mapster;

namespace HMS.Application.Services;

public class GuestService : IGuestService
{
    private readonly IGuestRepository _guestRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IPasswordHasher _passwordHasher;

    public GuestService(
        IGuestRepository guestRepository,
        IReservationRepository reservationRepository,
        IPasswordHasher passwordHasher)
    {
        _guestRepository = guestRepository;
        _reservationRepository = reservationRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<GuestResponseDto>> RegisterAsync(RegisterGuestDto dto)
    {
        var personalNumberTaken = await _guestRepository.ExistsAsync(g => g.PersonalNumber == dto.PersonalNumber);
        if (personalNumberTaken)
            return ApiResponse<GuestResponseDto>.Fail("A guest with this personal number already exists.");

        var phoneTaken = await _guestRepository.ExistsAsync(g => g.PhoneNumber == dto.PhoneNumber);
        if (phoneTaken)
            return ApiResponse<GuestResponseDto>.Fail("A guest with this phone number already exists.");

        var emailTaken = await _guestRepository.ExistsAsync(g => g.Email == dto.Email);
        if (emailTaken)
            return ApiResponse<GuestResponseDto>.Fail("A guest with this email already exists.");

        var guest = dto.Adapt<Domain.Entities.Guest>();
        guest.PasswordHash = _passwordHasher.Hash(dto.Password);

        await _guestRepository.AddAsync(guest);
        await _guestRepository.SaveAsync();
        return ApiResponse<GuestResponseDto>.Ok(guest.Adapt<GuestResponseDto>(), "Guest registered successfully.");
    }

    public async Task<ApiResponse<GuestResponseDto>> UpdateAsync(int id, UpdateGuestDto dto)
    {
        var guest = await _guestRepository.GetAsync(g => g.Id == id);
        if (guest is null)
            return ApiResponse<GuestResponseDto>.Fail("Guest not found.");

        guest.FirstName = dto.FirstName;
        guest.LastName = dto.LastName;
        guest.PhoneNumber = dto.PhoneNumber;
        _guestRepository.Update(guest);
        await _guestRepository.SaveAsync();
        return ApiResponse<GuestResponseDto>.Ok(guest.Adapt<GuestResponseDto>(), "Guest updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var guest = await _guestRepository.GetAsync(g => g.Id == id);
        if (guest is null)
            return ApiResponse<bool>.Fail("Guest not found.");

        var hasActiveReservations = await _reservationRepository.ExistsAsync(r =>
            r.GuestId == id && r.CheckOutDate >= DateTime.UtcNow.Date);
        if (hasActiveReservations)
            return ApiResponse<bool>.Fail("Cannot delete guest: they have active or upcoming reservations.");

        _guestRepository.Remove(guest);
        await _guestRepository.SaveAsync();
        return ApiResponse<bool>.Ok(true, "Guest deleted successfully.");
    }

    public async Task<ApiResponse<GuestResponseDto>> GetByIdAsync(int id)
    {
        var guest = await _guestRepository.GetAsync(g => g.Id == id, tracking: false);
        if (guest is null)
            return ApiResponse<GuestResponseDto>.Fail("Guest not found.");
        return ApiResponse<GuestResponseDto>.Ok(guest.Adapt<GuestResponseDto>());
    }
}
