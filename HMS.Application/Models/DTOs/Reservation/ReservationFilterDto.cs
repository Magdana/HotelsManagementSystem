namespace HMS.Application.DTOs.Reservation;

public class ReservationFilterDto
{
    public int? HotelId { get; set; }
    public int? GuestId { get; set; }
    public int? RoomId { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public bool? Active { get; set; }
}
