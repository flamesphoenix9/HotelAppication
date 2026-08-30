using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class RoomTypePriceRepo(AppDbContext appDbContext) : IRoomTypePriceRepo
    {
        private readonly AppDbContext _dbContext = appDbContext;

        public void Add(RoomTypePrice roomTypePrice, CancellationToken cancellationToken = default)
        {
            _dbContext.Add(roomTypePrice);
        }

        public async Task<IReadOnlyDictionary<RoomCategory, decimal>> GetAllActiveRoomTypePrice(CancellationToken cancellationToken = default)
        {
            return await _dbContext.RoomTypePrices.AsNoTracking().ToDictionaryAsync(rtp => rtp.Category, rtp => rtp.BasePricePerNight, cancellationToken);
        }

        public async Task<IReadOnlyCollection<RoomTypePrice>> GetAllRoomTypePricesAsList(CancellationToken cancellationToken = default)
        {
            return await _dbContext.RoomTypePrices.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<RoomTypePrice?> GetByIdAsync(Guid roomTypePriceId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RoomTypePrices.FindAsync(roomTypePriceId);
        }

        public void Update(RoomTypePrice roomTypePrice)
        {
            _dbContext.Update(roomTypePrice);
        }
    }
}