using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IReservationService
    {
        Task<Response<ReservationResponseDto>> GetReservationByIdAsync(Guid reservationId, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default);
        Task<Response<ReservationResponseDto>> CheckInReservationAsyc(Guid reservationId, ReservationCheckInRequestDto reserationCheckInRequestDto, CancellationToken cancellationToken = default);
        Task<Response<ReservationResponseDto>> CheckOutReservatonAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task<Response<ReservationResponseDto>> UpdateAssignedRoomAsync(Guid reservationId, CancellationToken cancellationToken = default);
    }
}