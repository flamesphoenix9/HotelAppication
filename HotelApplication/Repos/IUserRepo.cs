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