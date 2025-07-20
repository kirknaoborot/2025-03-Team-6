namespace AuthService.Contracts
{
    public record AuthResponse(string FullName, string IsActive, string Status, DateTime? LastSeen, string Role);
}
