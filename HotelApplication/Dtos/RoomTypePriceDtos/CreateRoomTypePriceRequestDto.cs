using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.RoomTypePriceDtos
{
    public record CreateRoomTypePriceRequestDto
    {
        public string Category { get; init; } = string.Empty;
        public decimal BasePricePerNight { get; set; }
    }
}