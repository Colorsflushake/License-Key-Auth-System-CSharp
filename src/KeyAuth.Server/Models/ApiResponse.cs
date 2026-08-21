namespace KeyAuth.Server.Models;

using System.Text.Json.Serialization;

public sealed class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; init; }

    public static ApiResponse Fail(string message) => new() { Success = false, Message = message };

    public static ApiResponse Success(object? data = null) => new()
    {
        Success = true,
        Message = "OK",
        Data = data
    };
}
