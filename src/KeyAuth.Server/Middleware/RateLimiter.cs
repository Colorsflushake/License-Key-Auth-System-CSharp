namespace KeyAuth.Server.Middleware;

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

public sealed class RateLimiter : IMiddleware
{
    private readonly ConcurrentDictionary<string, ClientBucket> _buckets = new();
    private readonly int _maxRequests = 60;
    private readonly TimeSpan _window = TimeSpan.FromMinutes(1);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var bucket = _buckets.GetOrAdd(clientIp, _ => new ClientBucket(DateTime.UtcNow));

        lock (bucket)
        {
            if (DateTime.UtcNow - bucket.WindowStart > _window)
            {
                bucket.WindowStart = DateTime.UtcNow;
                bucket.RequestCount = 0;
            }

            bucket.RequestCount++;

            if (bucket.RequestCount > _maxRequests)
            {
                context.Response.StatusCode = 429;
                context.Response.Headers["Retry-After"] = "60";
                return;
            }
        }

        await next(context);
    }

    private sealed class ClientBucket
    {
        public DateTime WindowStart { get; set; }
        public int RequestCount { get; set; }

        public ClientBucket(DateTime start)
        {
            WindowStart = start;
            RequestCount = 0;
        }
    }
}
