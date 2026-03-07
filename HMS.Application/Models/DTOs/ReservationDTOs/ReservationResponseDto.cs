namespace HMS.Application.DTOs.Reservation;

public class ReservationResponseDto
{
    public int Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int? GuestId { get; set; }
    public List<RoomSummaryDto> Rooms { get; set; } = new();
}

public class RoomSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
}
