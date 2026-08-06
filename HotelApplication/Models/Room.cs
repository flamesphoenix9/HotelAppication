using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Models
{
    public enum RoomCategory
    {
        Standard = 1,
        Executive = 2
    }
    [Index(nameof(RoonNumber), IsUnique =true)]
    public class Room : EntityBase
    {
        [Required]
        public required string RoonNumber { get; set; }
        public RoomCategory RoomCategory { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsAvailable { get; set; }

        public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
    }
}