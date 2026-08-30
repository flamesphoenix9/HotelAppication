using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class RoomDateAllocationRepo(AppDbContext dbContext):IRoomDateAllocationRepo
    {
        private readonly AppDbContext _context = dbContext;

        public void Add(RoomDateAllocation roomDateAllcation)
        {
            _context.RoomDateAllocations.Add(roomDateAllcation);
        }

        public async Task DeleteAllocationRange(Guid reservationId, CancellationToken cancellationToken = default)
        {
            var allocations = await _context.RoomDateAllocations.Where(r => r.ReservationId == reservationId).ToListAsync(cancellationToken);
            _context.RoomDateAllocations.RemoveRange(allocations);
        }
    }
}