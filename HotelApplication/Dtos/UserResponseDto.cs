namespace HotelApplication.Dtos
{
    public record UserResponseDto
    {
        public Guid UserId { get; init; }
        public string Username { get; init; } = string.Empty;
        public string EmailAddress { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string UserRole { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt{ get; init; }
    }
}