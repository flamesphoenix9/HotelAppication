using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Models
{
    public enum RoomCategory
    {
        Standard = 1,
        Executive = 2
    }
    
    public enum RoomStatus
    {
        Available,
        Occupied,
        Dirty,
        UnderMaintenance
    }
    [Index(nameof(RoomNumber), IsUnique = true)]
    public class Room : EntityBase
    {
        [Required]
        public string RoomNumber { get; set; } = string.Empty;
        [Required]
        public RoomCategory RoomCategory { get; set; }
        public bool IsOccupied => Status == RoomStatus.Occupied;
        public bool IsAvailable => Status == RoomStatus.Available;
        [Required]
        public RoomStatus Status { get; private set; } = RoomStatus.Available;
        public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

        public void CheckIn()
        {
            if (Status != RoomStatus.Available)
            {
                throw new InvalidOperationException(
                    $"Cannot check in to Room {RoomNumber}, Current status {Status}"
                );
            }
            Status = RoomStatus.Occupied;
        }
        public void CheckOut()
        {
            if (Status != RoomStatus.Occupied)
            {
                throw new InvalidOperationException(
                    $"Cannot check out of Room {RoomNumber}, Room is not Occupied");
            }
            Status = RoomStatus.Dirty;
        }
        public void MarkAvailable()
        {
            if (Status == RoomStatus.Occupied)
            {
                throw new InvalidOperationException(
                    $"Room {RoomNumber} is occupied, cannot mark room as available"
                );
            }
            Status = RoomStatus.Available;
        }
        public void MarkForMaintenance()
        {
            if (Status == RoomStatus.Occupied)
            {
                throw new InvalidOperationException(
                    $"Cannot place room {RoomNumber} on maintenance when room is occupied"
                );
            }
            Status = RoomStatus.UnderMaintenance;
        }
        
        public void MarkDirty()
        {
            if (Status== RoomStatus.Occupied)
            {
                throw new InvalidOperationException(
                    $"Cannot mark room {RoomNumber} dirty, room is Occupied"
                );
            }
            Status = RoomStatus.Dirty;
        }
    }
}
