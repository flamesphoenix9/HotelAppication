using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.RoomDtos
{
    public record UpdateRoomStatusDto
    {
        public Guid roomId { get; init; }
        public string roomStatus { get; init; } = string.Empty;
    }
}