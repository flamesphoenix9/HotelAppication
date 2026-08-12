using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos
{
    public record AdminCreateUserRequestDto
    {
        public required string Username { get; init; } 
        public required string EmailAddress { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Password { get; init; }
        public required string Role { get; init; }
    }
}