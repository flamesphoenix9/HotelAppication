using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos
{
    public class AdminViewUserResponseDto
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; } 
        public DateTime? UpdatedAt { get; init; }
        public bool IsDeleted { get; init; } 
        public DateTime? DeletedAt { get; init; }
        public string Username { get; init; } = string.Empty;
        public string EmailAddress { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string UserRole { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
    }
}