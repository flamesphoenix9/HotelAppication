using System.ComponentModel.DataAnnotations;

namespace HotelApplication.Dtos
{
    public record CreateUserRequestDto
    {
        public required string Username { get; init; } = string.Empty;
        public required string EmailAddress { get; init; } = string.Empty;
        public required string FirstName { get; init; } = string.Empty;
        public required string LastName { get; init; } = string.Empty;
        public required string Password { get; init; } = string.Empty;
    }
}