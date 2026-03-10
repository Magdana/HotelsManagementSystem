using HMS.Application.DTOs.Manager;
using HMS.Application.Interfaces;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Models;
using Mapster;

namespace HMS.Application.Services;

public class AdminService : IAdminService
{
    private readonly IManagerRepository _managerRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AdminService(IManagerRepository managerRepository, IPasswordHasher passwordHasher)
    {
        _managerRepository = managerRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<AdminResponseDto>> RegisterAsync(RegisterAdminDto dto)
    {
        var emailTaken = await _managerRepository.ExistsAsync(m => m.Email == dto.Email);
        if (emailTaken)
            return ApiResponse<AdminResponseDto>.Fail("An admin with this email already exists.");

        var personalNumberTaken = await _managerRepository.ExistsAsync(m => m.PersonalNumber == dto.PersonalNumber);
        if (personalNumberTaken)
            return ApiResponse<AdminResponseDto>.Fail("An admin with this personal number already exists.");

        var admin = dto.Adapt<Domain.Entities.Manager>();
        admin.PasswordHash = _passwordHasher.Hash(dto.Password);
        admin.Role = "Admin";
        admin.HotelId = null;

        await _managerRepository.AddAsync(admin);
        await _managerRepository.SaveAsync();
        return ApiResponse<AdminResponseDto>.Ok(admin.Adapt<AdminResponseDto>(), "Admin registered successfully.");
    }

    public async Task<ApiResponse<AdminResponseDto>> UpdateAsync(int adminId, UpdateAdminDto dto)
    {
        var admin = await _managerRepository.GetAsync(m => m.Id == adminId && m.Role == "Admin");
        if (admin is null)
            return ApiResponse<AdminResponseDto>.Fail("Admin not found.");

        admin.FirstName = dto.FirstName;
        admin.LastName = dto.LastName;
        admin.PhoneNumber = dto.PhoneNumber;
        _managerRepository.Update(admin);
        await _managerRepository.SaveAsync();
        return ApiResponse<AdminResponseDto>.Ok(admin.Adapt<AdminResponseDto>(), "Admin updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int adminId)
    {
        var admin = await _managerRepository.GetAsync(m => m.Id == adminId && m.Role == "Admin");
        if (admin is null)
            return ApiResponse<bool>.Fail("Admin not found.");

        var hasOtherAdmins = await _managerRepository.ExistsAsync(m => m.Role == "Admin" && m.Id != adminId);
        if (!hasOtherAdmins)
            return ApiResponse<bool>.Fail("Cannot delete the only admin.");

        _managerRepository.Remove(admin);
        await _managerRepository.SaveAsync();
        return ApiResponse<bool>.Ok(true, "Admin deleted successfully.");
    }

    public async Task<ApiResponse<List<AdminResponseDto>>> GetAllAsync()
    {
        var (items, _) = await _managerRepository.GetAllAsync(
            filter: m => m.Role == "Admin",
            tracking: false);
        return ApiResponse<List<AdminResponseDto>>.Ok(items.Adapt<List<AdminResponseDto>>());
    }

    public async Task<ApiResponse<AdminResponseDto>> GetByIdAsync(int adminId)
    {
        var admin = await _managerRepository.GetAsync(m => m.Id == adminId && m.Role == "Admin", tracking: false);
        if (admin is null)
            return ApiResponse<AdminResponseDto>.Fail("Admin not found.");
        return ApiResponse<AdminResponseDto>.Ok(admin.Adapt<AdminResponseDto>());
    }
}
