using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IUserRepo
    {
        Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
        Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
        Task<int> SaveUserAsync(CancellationToken cancellationToken);
    }
}