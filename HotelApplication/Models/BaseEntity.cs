using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Models
{
    public abstract class BaseEntity
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