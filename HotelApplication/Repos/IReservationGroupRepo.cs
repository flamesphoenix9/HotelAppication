using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IReservationGroupRepo
    {
        Task<ReservationGroup?> GetReservationGroupByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void AddReservationGroup(ReservationGroup reservationGroup);
        Task<ReservationGroup?> GetReservationGroupByCodeAsync(string reservationGrouCode, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<ReservationGroup>> GetReservationGroupByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<ReservationGroup>> GetReservationGroupsByDateCreatedAsync(DateTime createdAt, CancellationToken cancellationToken = default);
        Task UpdateReservationGroupAsync(ReservationGroup reservationGroup, CancellationToken cancellationToken = default);
    }
}