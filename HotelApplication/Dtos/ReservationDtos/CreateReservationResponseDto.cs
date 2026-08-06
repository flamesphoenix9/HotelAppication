namespace HotelApplication.Dtos.ReservationDtos
{
    public record CreateReservationResponseDto
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string RequestedRoomCategory { get; init; } = string.Empty;
        public DateTime ExpectedCheckIn { get; init; }
        public DateTime ExpectedCheckOut { get; init; }
        public int NumberOfDays { get; init; }
        public string ReservationStatus { get; init; } = string.Empty;

    }
}