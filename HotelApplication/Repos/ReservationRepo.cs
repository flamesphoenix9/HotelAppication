using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class ReservationRepo(AppDbContext appDbContext) : IReservationRepo
    {
        private readonly AppDbContext _dbContext = appDbContext;
        public async Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Reservations.AsNoTracking().Include(r=>r.ReservationGroup).FirstOrDefaultAsync(r => !r.IsDeleted && r.Id == reservationId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Reservation>> StaffGetAllReservationsAsync(CancellationToken cancellationToken = default)
        {
            var reservations = await _dbContext.Reservations.AsNoTracking().OrderByDescending(rsv=>rsv.ExpectedCheckIn).ToListAsync();
            return reservations;
        }

        public async Task UpdateReservationAsync(Reservation reservation, CancellationToken cancellationToken = default)
        {
            _dbContext.Reservations.Update(reservation);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateReservationRangeAsyncc(List<Reservation> reservations, CancellationToken cancellationToken)
        {
            _dbContext.UpdateRange(reservations);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
    }
}