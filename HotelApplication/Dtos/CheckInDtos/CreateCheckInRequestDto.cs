using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.CheckInDtos
{
    public record CreateCheckInRequestDto
    {
        public required Guid ReservationId { get; init; }
        public Guid RoomId { get; init; }
        public DateTime CheckInTime { get;init; } = DateTime.UtcNow;
    }
}