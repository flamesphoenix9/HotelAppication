namespace HotelApplication.Dtos.ReservationDtos
{
    public record CreateReservationRequestDto
    {
        public required string ReservationCategory { get; init; }
        public required DateOnly ExpectedCheckIn { get; init; }
        public required DateOnly ExpectedCheckOut { get; init; }
    }
}