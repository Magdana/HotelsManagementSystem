using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Room;

public class UpdateRoomDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public double Price { get; set; }
}
