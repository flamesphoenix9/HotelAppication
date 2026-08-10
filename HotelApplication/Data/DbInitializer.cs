using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;
using HotelApplication.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Data
{
    public class DbInitializer
    {
        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken= default)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var dbcontext = services.GetRequiredService<AppDbContext>();
                var authService = services.GetRequiredService<IAuthService>();
                var configService = services.GetRequiredService<IConfiguration>();
                var seedPassword = configService.GetValue<string>("SeedPassword")!;
                var checkPassword = authService.PasswordChecker(seedPassword);
                if (!checkPassword.IsValid)
                {
                    throw new InvalidOperationException($"SeedPasswordError {checkPassword.Errors}");
                }
                await SeedUsersAsync(dbcontext, seedPassword, authService, cancellationToken);
                await SeedRoomsAsync(dbcontext, cancellationToken);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static async Task SeedUsersAsync(AppDbContext appDbContext, string seedPassword, IAuthService authService, CancellationToken cancellationToken = default)
        {
            if (!await appDbContext.Users.AnyAsync(cancellationToken))
            {
                var users = new List<User>()
                {
                   new()
                    {
                        EmailAddress = "customer@example.com",
                        Username = "customer",
                        FirstName = "Standard",
                        LastName = "User",
                        Role = Roles.Customer,
                    },
                    new()
                    {
                        EmailAddress = "admin@example.com",
                        Username = "admin",
                        FirstName = "Standard",
                        LastName = "Admin",
                        Role = Roles.Admin,
                    },
                    new()
                    {
                        EmailAddress = "manager@example.com",
                        Username = "manager",
                        FirstName = "Standard",
                        LastName = "manager",
                        Role = Roles.Manager,
                    },
                       new()
                    {
                        EmailAddress = "receptionistuser@example.com",
                        Username = "receptionist",
                        FirstName = "Standard",
                        LastName = "Receptionist",
                        Role = Roles.Receptionist,
                    }
                };
                foreach (var user in users)
                {
                    user.PasswordHash = authService.HashPassword(seedPassword, user);
                }
                await appDbContext.Users.AddRangeAsync(users, cancellationToken);
                await appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
        public static async Task SeedRoomsAsync(AppDbContext appDbContext, CancellationToken cancellationToken=default)
        {
            if(!await appDbContext.Rooms.AnyAsync(cancellationToken))
            {
                var rooms = new List<Room>();
                for (int i = 100; i < 106; i++)
                {
                    rooms.Add(new Room
                    {
                        RoomNumber = $"{i}",
                        RoomCategory = RoomCategory.Standard
                    });
                }
                for (int i = 200; i < 206; i++)
                {
                    rooms.Add(new Room
                    {
                        RoomNumber = $"{i}",
                        RoomCategory = RoomCategory.Executive
                    });
                }
                await appDbContext.Rooms.AddRangeAsync(rooms, cancellationToken);
                await appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}