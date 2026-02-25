using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Hotel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }= string.Empty;
    [Range(1, 5)]
    public int Rating { get; set; }
    [Required]
    public string Country { get; set; }= string.Empty;
    [Required]
    public string City { get; set; }= string.Empty;
    [Required]
    public string Address { get; set; }= string.Empty;
    public List<Manager>? Managers { get; set; }
    public List<Room>? Rooms { get; set; }
}
