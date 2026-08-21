namespace KeyAuth.Server.Config;

public sealed class ServerConfig
{
    public string DatabasePath { get; init; } = "keys.db";
    public int Port { get; init; } = 5000;
    public string AdminToken { get; init; } = "change-me-in-production";
    public int MaxRequestsPerMinute { get; init; } = 60;
    public TimeSpan SessionDuration { get; init; } = TimeSpan.FromHours(12);
    public bool RequireHttps { get; init; } = true;

    public static ServerConfig Default => new();
}
