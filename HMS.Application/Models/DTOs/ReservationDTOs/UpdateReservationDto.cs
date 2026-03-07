using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Reservation;

public class UpdateReservationDto
{
    [Required]
    public DateTime CheckInDate { get; set; }
    [Required]
    public DateTime CheckOutDate { get; set; }
}
