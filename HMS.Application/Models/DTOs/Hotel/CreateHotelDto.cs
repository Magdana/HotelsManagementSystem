using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Hotel;

public class CreateHotelDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Range(1, 5)]
    public int Rating { get; set; }
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
}
