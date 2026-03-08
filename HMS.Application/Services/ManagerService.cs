using HMS.Application.DTOs.Manager;
using HMS.Application.Interfaces;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Models;
using Mapster;

namespace HMS.Application.Services;

public class ManagerService : IManagerService
{
    private readonly IManagerRepository _managerRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ManagerService(
        IManagerRepository managerRepository,
        IHotelRepository hotelRepository,
        IPasswordHasher passwordHasher)
    {
        _managerRepository = managerRepository;
        _hotelRepository = hotelRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<ManagerResponseDto>> RegisterAsync(int hotelId, RegisterManagerDto dto)
    {
        var hotelExists = await _hotelRepository.ExistsAsync(h => h.Id == hotelId);
        if (!hotelExists)
            return ApiResponse<ManagerResponseDto>.Fail("Hotel not found.");

        var emailTaken = await _managerRepository.ExistsAsync(m => m.Email == dto.Email);
        if (emailTaken)
            return ApiResponse<ManagerResponseDto>.Fail("A manager with this email already exists.");

        var personalNumberTaken = await _managerRepository.ExistsAsync(m => m.PersonalNumber == dto.PersonalNumber);
        if (personalNumberTaken)
            return ApiResponse<ManagerResponseDto>.Fail("A manager with this personal number already exists.");

        var manager = dto.Adapt<Domain.Entities.Manager>();
        manager.HotelId = hotelId;
        manager.PasswordHash = _passwordHasher.Hash(dto.Password);
        manager.Role = "Manager";

        await _managerRepository.AddAsync(manager);
        await _managerRepository.SaveAsync();
        return ApiResponse<ManagerResponseDto>.Ok(manager.Adapt<ManagerResponseDto>(), "Manager registered successfully.");
    }

    public async Task<ApiResponse<ManagerResponseDto>> UpdateAsync(int hotelId, int managerId, UpdateManagerDto dto)
    {
        var manager = await _managerRepository.GetAsync(m => m.Id == managerId && m.HotelId == hotelId);
        if (manager is null)
            return ApiResponse<ManagerResponseDto>.Fail("Manager not found.");

        manager.FirstName = dto.FirstName;
        manager.LastName = dto.LastName;
        manager.PhoneNumber = dto.PhoneNumber;
        _managerRepository.Update(manager);
        await _managerRepository.SaveAsync();
        return ApiResponse<ManagerResponseDto>.Ok(manager.Adapt<ManagerResponseDto>(), "Manager updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int hotelId, int managerId)
    {
        var manager = await _managerRepository.GetAsync(m => m.Id == managerId && m.HotelId == hotelId);
        if (manager is null)
            return ApiResponse<bool>.Fail("Manager not found.");

        var hasOtherManagers = await _managerRepository.ExistsAsync(m => m.HotelId == hotelId && m.Id != managerId);
        if (!hasOtherManagers)
            return ApiResponse<bool>.Fail("Cannot delete the only manager of a hotel.");

        _managerRepository.Remove(manager);
        await _managerRepository.SaveAsync();
        return ApiResponse<bool>.Ok(true, "Manager deleted successfully.");
    }

    public async Task<ApiResponse<List<ManagerResponseDto>>> GetByHotelIdAsync(int hotelId)
    {
        var (items, _) = await _managerRepository.GetAllAsync(
            filter: m => m.HotelId == hotelId,
            tracking: false);
        return ApiResponse<List<ManagerResponseDto>>.Ok(items.Adapt<List<ManagerResponseDto>>());
    }

    public async Task<ApiResponse<ManagerResponseDto>> GetByIdAsync(int hotelId, int managerId)
    {
        var manager = await _managerRepository.GetAsync(m => m.Id == managerId && m.HotelId == hotelId, tracking: false);
        if (manager is null)
            return ApiResponse<ManagerResponseDto>.Fail("Manager not found.");
        return ApiResponse<ManagerResponseDto>.Ok(manager.Adapt<ManagerResponseDto>());
    }
}
