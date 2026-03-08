namespace HMS.Application.DTOs.Room;

public class RoomFilterDto
{
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
