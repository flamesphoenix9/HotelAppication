using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface ICheckInRepo
    {
        Task<CheckIn> CreateCheckInAsync(CheckIn checkIn, CancellationToken cancellationToken= default);
        Task<ICollection<CheckIn>> GetCheckInsByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken= default);
        Task<CheckIn?> GetCheckInByIdAsync(Guid checkInId, CancellationToken cancellationToken= default);
        Task UpdateCheckInAsync(CheckIn checkIn, CancellationToken cancellationToken= default);
        Task<CheckIn?> GetActiveCheckInByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<CheckIn>> GetAllActiveCheckInsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<CheckIn>> GetRoomCheckInHistoryAsync(string roomNumber, CancellationToken cancellationToken=default);
        Task<IReadOnlyCollection<CheckIn>> GetCheckInsByUsernameAsync(string username, CancellationToken cancellationToken=default);
    }
}