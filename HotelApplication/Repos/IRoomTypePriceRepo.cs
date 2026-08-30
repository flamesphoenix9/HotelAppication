using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IRoomTypePriceRepo
    {
        Task<IReadOnlyDictionary<RoomCategory, decimal>> GetAllActiveRoomTypePrice(CancellationToken cancellationToken = default);
        void Add(RoomTypePrice roomTypePrice, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<RoomTypePrice>> GetAllRoomTypePricesAsList(CancellationToken cancellationToken = default);
        Task<RoomTypePrice?> GetByIdAsync(Guid roomTypePriceId, CancellationToken cancellationToken = default);
        void Update(RoomTypePrice roomTypePrice);
    }
}