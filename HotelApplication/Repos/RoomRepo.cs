using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class RoomRepo(AppDbContext appDbContext):IRoomRepo
    {
        private readonly AppDbContext _dbContext = appDbContext;

        public async Task<Room> CreateRoomAsync(Room room, CancellationToken cancellationToken = default)
        {
             var createdRoom = _dbContext.Rooms.Add(room);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return createdRoom.Entity;
        }

        public async Task<IReadOnlyCollection<Room>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
        {
            var rooms = await _dbContext.Rooms.AsNoTracking().Where(r => !r.IsDeleted).OrderBy(r=>r.RoomNumber).ToListAsync(cancellationToken);
            return rooms;
        }

        public async Task<IReadOnlyCollection<Room>> GetAvalaibleRoomsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Rooms.AsNoTracking().Where(r => r.Status== RoomStatus.Available && !r.IsDeleted).OrderBy(r=>r.RoomNumber).ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(r => !r.IsDeleted && r.Id == roomId, cancellationToken);
        }

        public async Task<Room?>GetByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(r => !r.IsDeleted && r.RoomNumber == roomNumber , cancellationToken);
        }

        public async Task UpdateRoomAsync(Room room, CancellationToken cancellationToken = default)
        {
            _dbContext.Rooms.Update(room);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
        
    }
}