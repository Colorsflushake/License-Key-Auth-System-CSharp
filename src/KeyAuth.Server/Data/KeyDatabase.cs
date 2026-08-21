namespace KeyAuth.Server.Data;

using System.Collections.Concurrent;
using KeyAuth.Server.Models;

public sealed class KeyDatabase
{
    private readonly ConcurrentDictionary<string, License> _licenses = new();

    public int Count => _licenses.Count;
    public int ActiveCount => _licenses.Values.Count(l => l.IsActive && l.ExpiresAt > DateTime.UtcNow);

    public Task<License?> GetLicenseAsync(string key)
    {
        _licenses.TryGetValue(key, out var license);
        return Task.FromResult(license);
    }

    public Task InsertLicenseAsync(License license)
    {
        _licenses[license.Key] = license;
        return Task.CompletedTask;
    }

    public Task<bool> DeactivateAsync(string key)
    {
        if (_licenses.TryGetValue(key, out var license))
        {
            _licenses[key] = license with { IsActive = false };
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ClearHwidAsync(string key)
    {
        if (_licenses.TryGetValue(key, out var license))
        {
            _licenses[key] = license with { BoundHwid = string.Empty };
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> SetHwidAsync(string id, string hwid)
    {
        var entry = _licenses.Values.FirstOrDefault(l => l.Id == id);
        if (entry is not null)
        {
            _licenses[entry.Key] = entry with { BoundHwid = hwid };
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
