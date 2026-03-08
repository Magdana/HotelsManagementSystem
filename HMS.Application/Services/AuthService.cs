using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HMS.Application.DTOs.Auth;
using HMS.Application.DTOs.Guest;
using HMS.Application.Interfaces;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Models;
using HMS.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IManagerRepository _managerRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IGuestService _guestService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IManagerRepository managerRepository,
        IGuestRepository guestRepository,
        IPasswordHasher passwordHasher,
        IGuestService guestService,
        IOptions<JwtSettings> jwtSettings)
    {
        _managerRepository = managerRepository;
        _guestRepository = guestRepository;
        _passwordHasher = passwordHasher;
        _guestService = guestService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var manager = await _managerRepository.GetAsync(m => m.Email == dto.Email);
        if (manager is not null && _passwordHasher.Verify(dto.Password, manager.PasswordHash))
        {
            var token = GenerateToken(manager.Id, manager.Email, manager.Role, manager.HotelId);
            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
            {
                Token = token,
                Email = manager.Email,
                Role = manager.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
            });
        }

        var guest = await _guestRepository.GetAsync(g => g.Email == dto.Email);
        if (guest is not null && _passwordHasher.Verify(dto.Password, guest.PasswordHash))
        {
            var token = GenerateToken(guest.Id, guest.Email, "Guest");
            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
            {
                Token = token,
                Email = guest.Email,
                Role = "Guest",
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
            });
        }

        return ApiResponse<AuthResponseDto>.Fail("Invalid email or password.");
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterGuestAsync(RegisterGuestDto dto)
    {
        var result = await _guestService.RegisterAsync(dto);
        if (!result.IsSuccess)
            return ApiResponse<AuthResponseDto>.Fail(result.Message);

        var guest = await _guestRepository.GetAsync(g => g.Email == dto.Email);
        var token = GenerateToken(guest!.Id, guest.Email, "Guest");
        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token = token,
            Email = guest.Email,
            Role = "Guest",
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        }, "Registration successful.");
    }

    private string GenerateToken(int id, string email, string role, int? hotelId = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, id.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (hotelId.HasValue)
            claims.Add(new Claim("hotelId", hotelId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
