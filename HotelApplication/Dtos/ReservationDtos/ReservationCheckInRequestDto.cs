using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.ReservationDtos
{
    public record ReservationCheckInRequestDto
    {
        public string CheckInNyName { get; init; } = string.Empty;
        public string CheckInByEmailAddress { get; init; } = string.Empty;
    }
}