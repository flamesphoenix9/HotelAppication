using System.ComponentModel.DataAnnotations;

namespace HotelApplication.Models
{
    public abstract class EntityBase
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        [Required]
        public bool IsDeleted { get; protected set; } = false;
        public DateTime? DeletedAt { get; protected set; }

        public void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }
        public virtual void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }
}