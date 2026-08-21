namespace KeyAuth.Client.Config;

using System.Text.Json;

public sealed class AuthConfig
{
    public required string ServerUrl { get; init; }
    public required string ApplicationId { get; init; }
    public required string AppSecret { get; init; }
    public required string AppVersion { get; init; }
    public string PublicKey { get; init; } = string.Empty;
    public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(10);
    public int MaxRetries { get; init; } = 3;
    public bool VerifyIntegrity { get; init; } = true;

    public static AuthConfig Load(string configPath)
    {
        var json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<AuthConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to parse auth configuration");
    }

    public static AuthConfig Default => new()
    {
        ServerUrl = "https://auth.example.com",
        ApplicationId = "app_default",
        AppSecret = "change_me",
        AppVersion = "1.0.0"
    };
}
