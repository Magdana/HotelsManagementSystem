using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class ReservationRoom
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }
  public int ReservationId { get; set; }
  public Reservation Reservation { get; set; } = null!;
  public int RoomId { get; set; }
  public Room Room { get; set; } = null!;
  public List<ReservationRoom>? ReservationRooms { get; set; }
}
