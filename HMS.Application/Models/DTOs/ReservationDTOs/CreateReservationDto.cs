using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Reservation;

public class CreateReservationDto
{
    [Required]
    public DateTime CheckInDate { get; set; }
    [Required]
    public DateTime CheckOutDate { get; set; }
    public int GuestId { get; set; }
    [Required, MinLength(1)]
    public List<int> RoomIds { get; set; } = new();
}
