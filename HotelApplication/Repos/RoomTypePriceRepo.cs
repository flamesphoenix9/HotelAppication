using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class RoomTypePriceRepo(AppDbContext appDbContext, ILogger<RoomTypePriceRepo> logger) : IRoomTypePriceRepo
    {
        private readonly AppDbContext _dbContext = appDbContext;
        private readonly ILogger<RoomTypePriceRepo> _logger = logger;

        public async Task<IReadOnlyDictionary<RoomCategory, decimal>> GetAllRoomTypePrice(CancellationToken cancellationToken = default)
        {
            return await _dbContext.RoomTypePrices.AsNoTracking().Where(rtp => rtp.IsActive).ToDictionaryAsync(rtp => rtp.Category, rtp => rtp.BasePricePerNight, cancellationToken);
        }
    }
}