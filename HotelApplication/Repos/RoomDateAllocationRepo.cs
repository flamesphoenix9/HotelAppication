using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public class RoomDateAllocationRepo(AppDbContext dbContext):IRoomDateAllocationRepo
    {
        private readonly AppDbContext _context = dbContext;

        public void Add(RoomDateAllocation roomDateAllcation)
        {
            _context.RoomDateAllocations.Add(roomDateAllcation);
        }
    }
}