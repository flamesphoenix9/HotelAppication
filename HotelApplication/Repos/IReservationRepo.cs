using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IReservationRepo
    {
        Task<Reservation> CreateReservationAsync(Reservation reservation, CancellationToken cancellationToken);
        Task<IEnumerable<Reservation>> GetAllReservationsAsync(string username, CancellationToken cancellationToken);
        Task<Reservation?> GetReservationByUserAsync(Guid reservationId, string username, CancellationToken cancellationToken);
        Task<Reservation?> GetUserReservationByStaffAsync(Guid reservationId, CancellationToken cancellationToken);
    }
}