using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IReservationService
    {
        Task<Response<CreateReservationResponseDto>> CreateReservationAsync(CreateReservationRequestDto requestDto,Guid userId, string username, CancellationToken cancellationToken);
        Task<Response<IEnumerable<ReservationResponseDto>>> UserViewAllReservationsAsync(string username, CancellationToken cancellationToken);
        Task<Response<ReservationResponseDto>> UserViewReservationAsync(Guid reservationId, string username, CancellationToken cancellationToken);
        // STAFF
        Task<Response<ReservationResponseDto>> StaffViewReservationAsync(Guid reservationId, CancellationToken cancellationToken);
    }
}