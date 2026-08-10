namespace HotelApplication.Models
{
    public class CheckIn : EntityBase
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public string RoomNumber => Room?.RoomNumber?? string.Empty;
        public Guid RoomId { get; init; }
        public Room Room { get; set; } = null!;

        public DateTime CheckInTime { get; init; } = DateTime.UtcNow;
        public DateTime? CheckOutTime { get; private set; }

        //To see if active
        public bool IsCheckInActive => CheckOutTime == null;

        public void CompleteCheckut()
        {
            if (!IsCheckInActive)
            {
                throw new InvalidOperationException("Guest has already checked out");
            }
            CheckOutTime = DateTime.UtcNow;
            Room.CheckOut();  
        }
        public override string ToString()
        {
            return $"{CheckInTime.Date} || {RoomNumber}";
        }
    }
}