namespace AuthService.Contracts
{
    public record AuthResponse(string Full_name, string Is_active, string Status, DateTime? Last_seen, string Role);
}
