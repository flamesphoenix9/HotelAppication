using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.RoomTypePriceDtos
{
    public class RoomTypePriceResponseDto
    {
        public Guid Id{ get; init; }
        public string Category { get; init; } = string.Empty;
        public decimal BasePricePerNight { get; init; }
        public bool IsActive { get; init; } = true;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? DeletedAt{ get; init; }
    }
}