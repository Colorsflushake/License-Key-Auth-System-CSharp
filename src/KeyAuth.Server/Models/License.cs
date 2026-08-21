namespace KeyAuth.Server.Models;

public sealed record License
{
    public required string Id { get; init; }
    public required string Key { get; init; }
    public required string Tier { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required int MaxDevices { get; init; }
    public required bool IsActive { get; init; }
    public required string BoundHwid { get; init; }
    public string? Notes { get; init; }

    public bool IsExpired => ExpiresAt < DateTime.UtcNow;
    public bool IsLifetime => ExpiresAt == DateTime.MaxValue;
}
