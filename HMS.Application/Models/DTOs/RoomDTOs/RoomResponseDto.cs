namespace HMS.Application.DTOs.Room;

public class RoomResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int? HotelId { get; set; }
}
