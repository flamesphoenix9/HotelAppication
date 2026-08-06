using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class ReservationRepo(AppDbContext appDbContext) : IReservationRepo
    {
        private readonly AppDbContext _dbContext = appDbContext;
        public async Task<Reservation> CreateReservationAsync(Reservation reservation, CancellationToken cancellationToken)
        {
            var result = _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync(string username, CancellationToken cancellationToken)
        {
            var reservations = await _dbContext.Reservations.AsNoTracking().Where(r => r.Username == username && !r.IsDeleted).ToListAsync(cancellationToken);
            return reservations;
        }
        public async Task<Reservation?> GetReservationByUserAsync(Guid reservationId, string username, CancellationToken cancellationToken)
        {
            var reservation = await _dbContext.Reservations.AsNoTracking().Include(r=>r.User).FirstOrDefaultAsync(r => r.Username == username && !r.IsDeleted && r.Id == reservationId);
            return reservation;
        }
        public async Task<Reservation?> GetUserReservationByStaffAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            var reservation = await _dbContext.Reservations.AsNoTracking().Include(r => r.User).FirstOrDefaultAsync(r =>  !r.IsDeleted && r.Id == reservationId, cancellationToken);
            return reservation;
        }
    }
}