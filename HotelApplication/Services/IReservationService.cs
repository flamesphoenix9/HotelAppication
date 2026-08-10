using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IReservationService
    {
        Task<Response<CreateReservationResponseDto>> CreateReservationAsync(CreateReservationRequestDto requestDto, Guid userId, string username, CancellationToken cancellationToken);
        Task<Response<IReadOnlyCollection<ReservationResponseDto>>> ViewAllReservationsAsync(string username, CancellationToken cancellationToken);
        Task<Response<ReservationResponseDto>> UserViewReservationAsync(Guid reservationId, string username, CancellationToken cancellationToken);
        Task<Response<string>> CancelReservationAsync(Guid reservationId,string username, CancellationToken cancellationToken = default);
        // STAFF
        Task<Response<ReservationResponseDto>> StaffViewReservationAsync(Guid reservationId, CancellationToken cancellationToken);
    }
}