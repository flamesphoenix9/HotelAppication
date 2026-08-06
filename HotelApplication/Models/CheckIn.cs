namespace HotelApplication.Models
{
    public class CheckIn:EntityBase
    {
        public Guid UserId { get; set; }
        public User User{ get; set; }
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
        public DateTime? CheckOutTime{ get; set; }
        public Guid ReceptionistId { get; set; }
        //To see if active
        public bool IsActive => CheckOutTime == null;

        public override string ToString()
        {
            return $"{User?.FullName} || {Room?.RoonNumber}";
        }
    }
}