using System.ComponentModel.DataAnnotations;

namespace HotelApplication.Dtos
{
    public record CreateUserRequestDto
    {
        [Required]
        public required string Username { get; init; } = string.Empty;
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