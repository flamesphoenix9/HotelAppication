using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Models;

namespace HotelApplication.Mappers
{
    public static class ReservationMapper
    {
        public static Reservation ToReservationMapper(this CreateReservationRequestDto createReservationRequestDto, Guid userId, string username)
        {
            return new Reservation
            {
                UserId = userId,
                RequestedRoomCategory = Enum.TryParse<RoomCategory>(createReservationRequestDto.RequestedRoomCategory, true, out var requestedRoomCategory)
                    ? requestedRoomCategory
                    : default,
                ExpectedCheckIn = createReservationRequestDto.ExpectedCheckIn,
                ExpectedCheckOut = createReservationRequestDto.ExpectedCheckOut,
                Username = username
            };
        }
      
    }
}