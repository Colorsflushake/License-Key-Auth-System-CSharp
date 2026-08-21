namespace KeyAuth.Client.Utils;

using System.Net.Http.Json;
using KeyAuth.Client.Crypto;

public sealed class HttpHelper : IDisposable
{
    private readonly HttpClient _client;
    private readonly RequestSigner _signer;

    public HttpHelper(string baseUrl, RequestSigner signer)
    {
        _signer = signer;
        _client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };
        _client.DefaultRequestHeaders.Add("User-Agent", "KeyAuth-Client/1.0");
    }

    public async Task<string?> PostAsync(string endpoint, Dictionary<string, string> data)
    {
        var signature = _signer.Sign(data);
        data["signature"] = signature;

        using var content = new FormUrlEncodedContent(data);
        using var response = await _client.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<T?> GetAsync<T>(string endpoint) where T : class
    {
        using var response = await _client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
