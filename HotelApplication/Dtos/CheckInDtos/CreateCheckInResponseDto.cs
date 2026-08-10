using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.CheckInDtos
{
    public record CreateCheckInResponseDto
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public Guid ReservationId { get; init; }
        public string RoomCategory { get; init; } = string.Empty;
        public string RoonNumber { get; init; } = string.Empty;
        public DateTime CheckInTime { get; init; } = DateTime.UtcNow;
         public bool IsActive { get; init; }
    }
}