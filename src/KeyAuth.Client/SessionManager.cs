namespace KeyAuth.Client;

using KeyAuth.Client.Models;

public sealed class SessionManager
{
    private readonly Dictionary<string, UserSession> _activeSessions = new();
    private readonly TimeSpan _sessionTimeout;
    private readonly Timer _cleanupTimer;

    public int ActiveCount => _activeSessions.Count;

    public SessionManager(TimeSpan sessionTimeout)
    {
        _sessionTimeout = sessionTimeout;
        _cleanupTimer = new Timer(CleanupExpired, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public void Register(UserSession session)
    {
        _activeSessions[session.SessionId] = session;
    }

    public UserSession? Get(string sessionId)
    {
        if (_activeSessions.TryGetValue(sessionId, out var session) && !session.IsExpired)
        {
            return session;
        }
        return null;
    }

    public bool Refresh(string sessionId)
    {
        if (_activeSessions.TryGetValue(sessionId, out var session))
        {
            var refreshed = session with { ExpiresAt = DateTime.UtcNow.Add(_sessionTimeout) };
            _activeSessions[sessionId] = refreshed;
            return true;
        }
        return false;
    }

    public void Revoke(string sessionId)
    {
        _activeSessions.Remove(sessionId);
    }

    private void CleanupExpired(object? state)
    {
        var expired = _activeSessions
            .Where(kvp => kvp.Value.IsExpired)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expired)
        {
            _activeSessions.Remove(key);
        }
    }
}
