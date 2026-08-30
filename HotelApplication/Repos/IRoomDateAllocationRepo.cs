using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IRoomDateAllocationRepo
    {
        void Add(RoomDateAllocation roomDateAllcation);
        Task DeleteAllocationRange(Guid reservationId, CancellationToken cancellationToken = default);
    }
}