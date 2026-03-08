namespace HMS.Application.DTOs.Hotel;

public class HotelFilterDto
{
    public string? Country { get; set; }
    public string? City { get; set; }
    public int? Rating { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
