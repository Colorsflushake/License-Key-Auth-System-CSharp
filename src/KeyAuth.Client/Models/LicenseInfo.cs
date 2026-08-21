namespace KeyAuth.Client.Models;

using System.Text.Json;

public sealed record LicenseInfo
{
    public required string LicenseKey { get; init; }
    public required string Username { get; init; }
    public required string BoundHwid { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required string Tier { get; init; }
    public string Signature { get; init; } = string.Empty;

    public bool IsLifetime => ExpiresAt == DateTime.MaxValue;
    public bool IsActive => ExpiresAt > DateTime.UtcNow;
    public TimeSpan TimeRemaining => ExpiresAt - DateTime.UtcNow;

    public static LicenseInfo? FromJson(string json)
    {
        return JsonSerializer.Deserialize<LicenseInfo>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}
