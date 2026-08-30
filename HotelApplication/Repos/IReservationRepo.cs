using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IReservationRepo
    {
        Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task UpdateReservationAsync(Reservation reservation, CancellationToken cancellationToken = default);
        Task UpdateReservationRangeAsyncc(List<Reservation> reservations, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Reservation>> StaffGetAllReservationsAsync(CancellationToken cancellationToken = default);
    }
}