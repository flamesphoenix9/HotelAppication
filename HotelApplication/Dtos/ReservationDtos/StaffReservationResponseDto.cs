namespace HotelApplication.Dtos.ReservationDtos
{
    public class StaffReservationResponseDto
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public Guid UserId { get; init; }
        public string RequestedRoomCategory { get; init; } = string.Empty;
        public DateTime ExpectedCheckIn { get; init; }
        public DateTime ExpectedCheckOut { get; init; }
        public int NumberOfDays { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string ReservationStatus { get; init; } = string.Empty;
    }
}