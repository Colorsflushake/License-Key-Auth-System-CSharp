namespace KeyAuth.Server.Data;

using System.Collections.Concurrent;

public sealed class UserRepository
{
    private readonly ConcurrentDictionary<string, UserRecord> _users = new();

    public Task<UserRecord?> GetByUsernameAsync(string username)
    {
        _users.TryGetValue(username, out var user);
        return Task.FromResult(user);
    }

    public Task CreateAsync(UserRecord user)
    {
        _users[user.Username] = user;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateLastLoginAsync(string username)
    {
        if (_users.TryGetValue(username, out var user))
        {
            _users[username] = user with { LastLogin = DateTime.UtcNow };
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public IEnumerable<UserRecord> GetAll() => _users.Values;
}

public sealed record UserRecord
{
    public required string Username { get; init; }
    public required string PasswordHash { get; init; }
    public required string Role { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; init; }
}
