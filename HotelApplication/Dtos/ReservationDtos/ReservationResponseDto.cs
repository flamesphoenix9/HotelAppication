namespace HotelApplication.Dtos.ReservationDtos
{
    public record ReservationResponseDto
    {
        public Guid Id { get; init; }
        public Guid ReservationGroupId { get; set; }
        public string ReservationCategory { get; init; } = string.Empty;
        public DateOnly ExpectedCheckIn { get; init; }
        public DateOnly ExpectedCheckOut { get; init; }
        public int NumberOfDays { get; init; }
        public string ReservationStatus { get; init; } = string.Empty;
        public string AssignedRoomId { get; init; } = string.Empty;
        public string? AssignedRoomNumber { get; init; }
        public decimal PricePerNight { get; init; }
        public decimal ReservationTotal { get; init; }
        public DateTime? ActualCheckInTime { get; init; }
        public DateTime? ActualCheckOutTime{ get; init; }
        public string? CheckInByName { get; init; }
        public string? CheckInByEmailAddress { get; init; }

    }
}