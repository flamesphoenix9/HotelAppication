using HotelApplication.Data;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repos
{
    public class UserRepo(AppDbContext appDbContext) : IUserRepo
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            var result = _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task<List<User>?> CheckUniqueEmailAndUsername(string email, string username, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            var normalised_username = username.ToUpperInvariant();
            var normalised_email = email.ToUpperInvariant();
            return await _appDbContext.Users.AsNoTracking().Where(u => u.Normalised_Email == normalised_email || u.Normalised_Username == normalised_username).ToListAsync(cancellationToken);
        }


        public async Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u=>u.Id ==userId, cancellationToken);
        }

        public Task<int> SaveUserAsync(CancellationToken cancellationToken)
        {
            return _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }
            var normalised_email = email.ToUpperInvariant();
            return await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Normalised_Email == normalised_email, cancellationToken);
        }

        public async Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var normalised_Username = username.ToUpperInvariant();
            return await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => !u.IsDeleted && u.Normalised_Username == normalised_Username);
        }

        public async Task<IReadOnlyCollection<User>> AdminGetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task UpdateUsersAsync(User user, CancellationToken cancellationToken = default)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}