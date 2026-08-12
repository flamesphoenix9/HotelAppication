using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class CheckInRepo(AppDbContext appDbContext):ICheckInRepo
    {
        private readonly AppDbContext _dbContext = appDbContext;
        public async Task<CheckIn> CreateCheckInAsync(CheckIn checkIn, CancellationToken cancellationToken= default)
        {
            var newCheckin = _dbContext.CheckIns.Add(checkIn);
            await _dbContext.SaveChangesAsync(cancellationToken);
           return (await GetCheckInByIdAsync(checkIn.Id, cancellationToken))!;
        }

        public async Task<CheckIn?> GetActiveCheckInByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            var activeCheckIn = await _dbContext.CheckIns.AsNoTracking().Include(c => c.Room).Include(c=>c.User).FirstOrDefaultAsync(c => !c.IsDeleted &&(c.CheckOutTime== null) && c.Room.RoomNumber == roomNumber, cancellationToken);
            return activeCheckIn;
        }

        public async Task<IReadOnlyCollection<CheckIn>> GetAllActiveCheckInsAsync(CancellationToken cancellationToken=default)
        {
            var activeCheckIns = await _dbContext.CheckIns.AsNoTracking().Include(c => c.Room).Include(c=>c.User).Where(c => !c.IsDeleted && (c.CheckOutTime==null)).ToListAsync(cancellationToken);
            return activeCheckIns;
        }

        public async Task<IReadOnlyCollection<CheckIn>> GetRoomCheckInHistoryAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            var allRoomCheckIns = await _dbContext.CheckIns.AsNoTracking().Include(r => r.Room).Include(c=>c.User).Where(r => r.Room.RoomNumber == roomNumber).ToListAsync(cancellationToken);
            return allRoomCheckIns;
        }

        public async Task<CheckIn?> GetCheckInByIdAsync(Guid checkInId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.CheckIns.AsNoTracking().Include(c => c.Room).Include(c=>c.User).FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == checkInId, cancellationToken);
        }
        
        public async Task<ICollection<CheckIn>> GetCheckInsByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken= default)
        {
            var roomCheckIns = await _dbContext.CheckIns.AsNoTracking().Include(c=>c.Room).Include(c=>c.User).Where(c => c.Room.RoomNumber == roomNumber ).OrderByDescending(c=>c.CheckInTime).ToListAsync(cancellationToken);
            return roomCheckIns;
        }

        public async Task<IReadOnlyCollection<CheckIn>> GetCheckInsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var normalised_Username = username.ToUpperInvariant();
            var userCeckIns = await _dbContext.CheckIns.AsNoTracking().Include(c => c.User).Include(c=>c.Room).Where(c => c.User.Normalised_Username== normalised_Username).OrderByDescending(c=>c.CheckInTime).ToListAsync(cancellationToken);
            return userCeckIns;
        }

        public async Task UpdateCheckInAsync(CheckIn checkIn, CancellationToken cancellationToken = default)
        {
            _dbContext.CheckIns.Update(checkIn);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
        
    }
}