namespace KeyAuth.Client.Models;

public sealed record UserSession
{
    public string SessionId { get; init; }
    public string Username { get; init; }
    public string HardwareId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public TimeSpan Remaining => ExpiresAt - DateTime.UtcNow;

    public UserSession(string username, string hardwareId, DateTime expiresAt)
    {
        SessionId = Guid.NewGuid().ToString("N");
        Username = username;
        HardwareId = hardwareId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }
}
