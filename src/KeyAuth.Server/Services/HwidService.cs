namespace KeyAuth.Server.Services;

using System.Collections.Concurrent;
using KeyAuth.Server.Data;

public sealed class HwidService
{
    private readonly KeyDatabase _database;
    private readonly ConcurrentDictionary<string, SessionEntry> _sessions = new();

    public HwidService(KeyDatabase database)
    {
        _database = database;
    }

    public async Task BindAsync(string licenseId, string hwid)
    {
        await _database.SetHwidAsync(licenseId, hwid);
    }

    public bool ValidateSession(string sessionId, string hwid)
    {
        if (_sessions.TryGetValue(sessionId, out var entry))
        {
            return entry.Hwid == hwid && entry.ExpiresAt > DateTime.UtcNow;
        }
        return false;
    }

    public string CreateSession(string licenseId, string hwid, TimeSpan duration)
    {
        var sessionId = Guid.NewGuid().ToString("N");
        _sessions[sessionId] = new SessionEntry(licenseId, hwid, DateTime.UtcNow.Add(duration));
        return sessionId;
    }

    public void RevokeSession(string sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
    }

    private sealed record SessionEntry(string LicenseId, string Hwid, DateTime ExpiresAt);
}
