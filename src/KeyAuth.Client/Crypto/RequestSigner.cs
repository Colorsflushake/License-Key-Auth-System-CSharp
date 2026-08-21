namespace KeyAuth.Client.Crypto;

using System.Security.Cryptography;
using System.Text;

public sealed class RequestSigner
{
    private readonly byte[] _secretKey;

    public RequestSigner(string secret)
    {
        _secretKey = Encoding.UTF8.GetBytes(secret);
    }

    public string Sign(Dictionary<string, string> parameters)
    {
        var sorted = parameters
            .Where(kvp => kvp.Key != "signature")
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{kvp.Key}={kvp.Value}");

        var payload = string.Join("&", sorted);
        return ComputeHmac(payload);
    }

    public bool Verify(Dictionary<string, string> parameters, string expectedSignature)
    {
        var computed = Sign(parameters);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(expectedSignature));
    }

    private string ComputeHmac(string data)
    {
        using var hmac = new HMACSHA256(_secretKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
