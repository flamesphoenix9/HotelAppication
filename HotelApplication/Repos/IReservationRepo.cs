using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IReservationRepo
    {
        Task<Reservation> CreateReservationAsync(Reservation reservation, CancellationToken cancellationToken= default);
        Task<IReadOnlyCollection<Reservation>> GetReservationsByUsernameAsync(string username, CancellationToken cancellationToken = default);
        
        Task<Reservation?> GetReservationByUserAsync(Guid reservationId, string username, CancellationToken cancellationToken= default);
        Task<Reservation?> GetUserReservationByStaffAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task UpdateReservationAsync(Reservation reservation, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Reservation>> StaffGetAllReservationsAsync(CancellationToken cancellationToken = default);
    }
}