using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Guest;

public class UpdateGuestDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
}
