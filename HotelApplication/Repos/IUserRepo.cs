using HotelApplication.Models;

namespace HotelApplication.Repos
{
    public interface IUserRepo
    {
        Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
        Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
;        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
        Task<List<User>?> CheckUniqueEmailAndUsername(string email,string username, CancellationToken cancellationToken);
        Task<int> SaveUserAsync(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<User>> AdminGetAllUsersAsync(CancellationToken cancellationToken = default);
        Task UpdateUsersAsync(User user, CancellationToken cancellationToken = default);
    }
}