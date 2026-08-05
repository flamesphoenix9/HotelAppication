using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos
{
    public record CreateUserRequestDto
    {
        [Required]
        public required string EmailAddress { get; init; } = string.Empty;
        [Required]
        public required string FirstName { get; init; } = string.Empty;
        [Required]
        public required string LastName { get; init; } = string.Empty;
        [Required]
        public required string Password { get; init; } = string.Empty;
    }
}