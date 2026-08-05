namespace HotelApplication.Dtos
{
    public record LoginRequestDto
    {
        public string EmailAddress { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}