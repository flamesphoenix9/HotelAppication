namespace HotelApplication.Utilities
{
    public record PasswordValidationResult
    {
        public bool IsValid { get; init; }
        public List<string> Errors { get; init; } = new();
    }
}