using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Hotel;

public class UpdateHotelDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Range(1, 5)]
    public int Rating { get; set; }
}
