namespace KeyAuth.Server;

using KeyAuth.Server.Config;
using KeyAuth.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ServerConfig>(sp =>
{
    var config = builder.Configuration.GetSection("Server").Get<ServerConfig>();
    return config ?? ServerConfig.Default;
});
builder.Services.AddSingleton<Services.KeyService>();
builder.Services.AddSingleton<Services.HwidService>();
builder.Services.AddSingleton<Data.KeyDatabase>();
builder.Services.AddSingleton<Data.UserRepository>();

var app = builder.Build();

app.UseMiddleware<RateLimiter>();
app.MapControllers();
app.Run();
