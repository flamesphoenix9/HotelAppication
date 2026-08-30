using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Models
{
    public class Reservation : EntityBase
    {
        //  === PARENT GROUP ===
        [Required]
        public Guid ReservationGroupId { get; set; }
        public ReservationGroup? ReservationGroup { get; set; } 

        //  === EXPECTED DATES ===
        public DateOnly ExpectedCheckIn { get; set; }
        public DateOnly ExpectedCheckOut { get; set; }
        [NotMapped]
        public int NumberOfDays => ExpectedCheckOut.DayNumber - ExpectedCheckIn.DayNumber;       

        //  === CHECK IN AND CHECKOUT AUDIT ===
        public DateTime? ActualCheckInTime { get; set; }
        public DateTime? ActualCheckOutTime{ get; set; }
        public string? CheckInByName { get; set; }
        public string? CheckInByEmailAddress { get; set; }
        
         // === sTATUS & ROOM ====
        public ReservationStatusEnum ReservationStatus { get; set; } = ReservationStatusEnum.Reserved;
        public RoomCategory ReservationCategory { get; set; }
        public Guid? AssignedRoomId { get; set; }
        public Room? AssignedRoom { get; set; }
        public string? AssignedRoomNumber { get; set; }
        
        //  === PRICING ===
        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }
        [NotMapped]
        public decimal ReservationTotal => PricePerNight * NumberOfDays;

        //  === NAVIGATION ===
        public ICollection<RoomDateAllocation> RoomDateAllocations { get; set; } = new List<RoomDateAllocation>();
        
        public void AssignRoomToReservation(Guid roomId, string roomNumber )
        {
            AssignedRoomId = roomId;
            AssignedRoomNumber = roomNumber;
        }
        public void CheckIn(string checkInByName, string checkInByEmailAddress, DateTime? checkInTime = null)
        {
            if (ReservationStatus != ReservationStatusEnum.Reserved)
            {
                throw new InvalidOperationException($"Cannoy check in reservation with status {ReservationStatus}");
            }
            CheckInByName = checkInByName;
            CheckInByEmailAddress = checkInByEmailAddress;
            ActualCheckInTime = checkInTime ?? DateTime.UtcNow;
            ReservationStatus = ReservationStatusEnum.CheckedIn;
        }
        public void CheckOut(DateTime? checkOutTime = null)
        {
            if (ReservationStatus != ReservationStatusEnum.CheckedIn)
            {
                throw new InvalidOperationException($"Cannot check out reservation;Status : {ReservationStatus}");
            }
            ReservationStatus = ReservationStatusEnum.CheckedOut;
            ActualCheckOutTime = checkOutTime ?? DateTime.UtcNow;
        }
        public void CancelReservation()
        {
            ReservationStatus = ReservationStatusEnum.Cancelled;
        }
    }
  
    
}