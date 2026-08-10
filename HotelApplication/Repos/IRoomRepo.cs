using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IRoomRepo
    {
        Task<Room> CreateRoomAsync(Room room, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Room>> GetAllRoomsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Room>> GetAvalaibleRoomsAsync(CancellationToken cancellationToken=default);
        Task<Room?> GetByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default);
        Task<Room?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
        Task UpdateRoomAsync(Room room, CancellationToken cancellationToken = default);
    }
}