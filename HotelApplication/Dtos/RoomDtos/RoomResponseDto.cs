using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.RoomDtos
{
    public record RoomResponseDto
    {
        public Guid Id { get; init; }
        public string RoomNumber { get; init; } = string.Empty;
        public string RoomCategory { get; init; } = string.Empty;
        public bool IsOccupied { get; init; }
        public bool IsAvailable{ get; init; }
        public string Status { get; init; } = string.Empty;
    }
}