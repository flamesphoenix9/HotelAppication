using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Models
{
    [Index(nameof(BookingCode), IsUnique = true)]
    public class Reservation : EntityBase
    {
        public Reservation()
        {
            NumberOfDays = (ExpectedCheckOut - ExpectedCheckIn).Days;
        }
        //BookingCode = User Email + random string 
        public string BookingCode { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; } = null;
        public RoomCategory RequestedRoomCategory { get; set; }
        public DateTime ExpectedCheckIn { get; set; }
        public DateTime ExpectedCheckOut { get; set; }
        public int NumberOfDays { get; }
        public ReservationStatus ReservationStatus { get; set; } = ReservationStatus.Confirmed;

    }

    public enum ReservationStatus
    {
        Confirmed = 1,
        Cancelled = 2,
        Expired=3
    }
}