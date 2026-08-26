using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.ReservationDtos;

namespace HotelApplication.Dtos.ReservationGroupDtos
{
    public record ReservationGroupResponseDto
    {
        public string ReservationGroupCode { get; init; } = string.Empty;
        public decimal AmountPayable { get; init; }
        public string ReservationGroupStatus { get; init; }=string.Empty;

        //  === USER ===
        public Guid UserId { get; init; }
        public string Username { get; init; } = string.Empty;
        public ICollection<ReservationResponseDto>? Reservations { get; init; } 
    }
}