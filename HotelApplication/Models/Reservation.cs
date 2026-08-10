using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Models
{
    public class Reservation : EntityBase
    {
        private string _userName = string.Empty;
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Username
        {
            get => _userName; set
            {
                _userName = value.ToUpperInvariant();
                Normalised_Username = value?.Trim().ToUpperInvariant() ?? string.Empty;
            }
        }
        public string Normalised_Username { get; set; } = string.Empty;
        public RoomCategory RequestedRoomCategory { get; set; }
        public DateTime ExpectedCheckIn { get; set; }
        public DateTime ExpectedCheckOut { get; set; }
        public int NumberOfDays => (ExpectedCheckOut.Date - ExpectedCheckIn.Date).Days;
        public ReservationStatus ReservationStatus { get; set; } = ReservationStatus.Confirmed;

    }
    public enum ReservationStatus
    {
        Confirmed = 1,
        CheckedIn = 2,
        Cancelled = 3
    }
}