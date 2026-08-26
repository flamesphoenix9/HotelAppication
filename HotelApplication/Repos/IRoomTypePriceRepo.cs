using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IRoomTypePriceRepo
    {
        Task<IReadOnlyDictionary<RoomCategory, decimal>> GetAllRoomTypePrice(CancellationToken cancellationToken=default);
    }
}