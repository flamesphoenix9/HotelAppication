using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.ReservationDtos;

namespace HotelApplication.Dtos.ReservationGroupDtos
{
    public record CreateReservationGroupRequestDto
    {
        public IReadOnlyCollection<CreateReservationRequestDto> CreateReservationsList { get; init; } = null!;
    }
}