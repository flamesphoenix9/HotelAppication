using HotelApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HotelApplication.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store Role Enum as text 
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

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