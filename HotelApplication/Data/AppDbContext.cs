using System.Security.Cryptography;
using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HotelApplication.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<ReservationGroup> ReservationGroups => Set<ReservationGroup>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomTypePrice> RoomTypePrices => Set<RoomTypePrice>();
        public DbSet<RoomDateAllocation> RoomDateAllocations => Set<RoomDateAllocation>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  Ensure Room cannot be booked twice on same day
            modelBuilder.Entity<RoomDateAllocation>().HasIndex(rda => new { rda.Date, rda.RoomId }, "RoomDateIndex").IsUnique();
            //  Ensure there are no two active prices for a category
            modelBuilder.Entity<RoomTypePrice>().HasIndex(rtp => new { rtp.Category, rtp.IsActive }).IsUnique();
            //  Ensure group codes are uniques
            modelBuilder.Entity<ReservationGroup>().HasIndex(rg => rg.ReservationGroupCode).IsUnique();

            //  Unique indexes in User
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Normalised_Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.EmailAddress).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Normalised_Email).IsUnique();

            // Store Role Enum as text 
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
            modelBuilder.Entity<Room>().Property(r => r.RoomCategory).HasConversion<string>();
            modelBuilder.Entity<Room>().Property(r => r.Status).HasConversion<string>();
            modelBuilder.Entity<Reservation>().Property(r => r.ReservationStatus).HasConversion<string>();
            modelBuilder.Entity<Reservation>().Property(r => r.ReservationCategory).HasConversion<string>();
            modelBuilder.Entity<ReservationGroup>().Property(rg => rg.ReservationGroupStatus).HasConversion<string>();
            modelBuilder.Entity<RoomTypePrice>().Property(rtp => rtp.Category).HasConversion<string>();            

            // Global configurations for all entities deriving from BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
                {
                    // 1. Force EF Core to use property setters for UpdatedAt
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(EntityBase.UpdatedAt))
                        .ValueGeneratedOnUpdate()
                        .Metadata.SetPropertyAccessMode(PropertyAccessMode.Property);

                    // 2. Prevent CreatedAt from being overwritten on SQL UPDATE statements
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(EntityBase.CreatedAt))
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                }
            }
        }
        //Trgger Touch() on update
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken= default)
        {
            var entries = ChangeTracker.Entries<EntityBase>();
            foreach (var entry in entries)
            {
                entry.Entity.Touch();       // updatedAt 
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}