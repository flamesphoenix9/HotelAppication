namespace HotelApplication.Dtos.ReservationDtos
{
    public record CreateReservationRequestDto
    {
        public required string RequestedRoomCategory { get; init; }
        public required DateTime ExpectedCheckIn { get; init; }
        public required DateTime ExpectedCheckOut { get; init; }
    }
}