using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Manager;

public class UpdateAdminDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
