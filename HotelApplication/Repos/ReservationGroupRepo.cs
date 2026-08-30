using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class ReservationGroupRepo(AppDbContext appDbContext):IReservationGroupRepo
    {
        private readonly AppDbContext _dbContext = appDbContext;
        public void AddReservationGroup(ReservationGroup reservationGroup)
        {
            _dbContext.ReservationGroups.Add(reservationGroup);
        }

        public async Task<ReservationGroup?> GetReservationGroupByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var reservationGroup = await _dbContext.ReservationGroups.AsNoTracking()
                .Include(rg=>rg.User).Include(rg=>rg.Reservations)
                .FirstOrDefaultAsync(rg => !rg.IsDeleted && rg.Id == id);
            return reservationGroup;
        }

        public async Task<IReadOnlyCollection<ReservationGroup>> GetReservationGroupByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var reservationGroups = await _dbContext.ReservationGroups.AsNoTracking().Include(rg=>rg.Reservations)
                .Where(rg => !rg.IsDeleted && rg.UserId == userId)
                .OrderByDescending(rg=>rg.CreatedAt).ToListAsync(cancellationToken);
            return reservationGroups;
        }

        public async Task<ReservationGroup?> GetReservationGroupByCodeAsync(string reservationGroupCode, CancellationToken cancellationToken = default)
        {
            var reservationGroup = await _dbContext.ReservationGroups.AsNoTracking()
                .Include(rg => rg.Reservations).Include(rg=>rg.User)
                .FirstOrDefaultAsync(rg => rg.ReservationGroupCode == reservationGroupCode, cancellationToken);
            return reservationGroup;
        }

        public async Task<IReadOnlyCollection<ReservationGroup>> GetReservationGroupsByDateCreatedAsync(DateTime createdAt, CancellationToken cancellationToken = default)
        {
            var reservationGroups = await _dbContext.ReservationGroups.AsNoTracking()
                .Include(rg => rg.User)
                .Where(rg => !rg.IsDeleted && rg.CreatedAt == createdAt).ToListAsync(cancellationToken);
            return reservationGroups;
        }

        public void UpdateReservationGroup(ReservationGroup reservationGroup)
        {
            _dbContext.ReservationGroups.Update(reservationGroup);
        }
    }
}