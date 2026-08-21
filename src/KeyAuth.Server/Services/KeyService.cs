namespace KeyAuth.Server.Services;

using System.Security.Cryptography;
using KeyAuth.Server.Data;
using KeyAuth.Server.Models;

public sealed class KeyService
{
    private readonly KeyDatabase _database;

    public KeyService(KeyDatabase database)
    {
        _database = database;
    }

    public async Task<License?> FindByKeyAsync(string key)
    {
        return await _database.GetLicenseAsync(key);
    }

    public async Task<License> GenerateAsync(string tier, TimeSpan duration, int maxDevices)
    {
        var key = GenerateKeyString();
        var license = new License
        {
            Id = Guid.NewGuid().ToString("N"),
            Key = key,
            Tier = tier,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(duration),
            MaxDevices = maxDevices,
            IsActive = true,
            BoundHwid = string.Empty
        };

        await _database.InsertLicenseAsync(license);
        return license;
    }

    public async Task<bool> RevokeAsync(string licenseKey)
    {
        return await _database.DeactivateAsync(licenseKey);
    }

    public async Task<bool> ResetHwidAsync(string licenseKey)
    {
        return await _database.ClearHwidAsync(licenseKey);
    }

    public object GetStatistics()
    {
        return new
        {
            TotalKeys = _database.Count,
            ActiveKeys = _database.ActiveCount,
            ExpiredKeys = _database.Count - _database.ActiveCount
        };
    }

    private static string GenerateKeyString()
    {
        var bytes = RandomNumberGenerator.GetBytes(20);
        var encoded = Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
        return $"KEY-{encoded[..6]}-{encoded[6..12]}-{encoded[12..18]}-{encoded[18..24]}".ToUpperInvariant();
    }
}
