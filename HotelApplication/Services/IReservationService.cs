using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IReservationService
    {
        Task<Response<CreateReservationResponseDto>> CreateReservationAsync(CreateReservationRequestDto requestDto, Guid userId, string username, CancellationToken cancellationToken);
        Task<Response<IReadOnlyCollection<ReservationResponseDto>>> GetReservationsByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<Response<ReservationResponseDto>> UserGetReservationByIdAsync(Guid reservationId, string username, CancellationToken cancellationToken);

        Task<Response<string>> CancelReservationAsync(Guid reservationId, string username, CancellationToken cancellationToken = default);
        // STAFF
        Task<Response<ReservationResponseDto>> StaffGetReservationByIdAsync(Guid reservationId, CancellationToken cancellationToken);
        Task<Response<IReadOnlyCollection<ReservationResponseDto>>> StaffGetAllReservationsAsync( CancellationToken cancellationToken);
    }
}