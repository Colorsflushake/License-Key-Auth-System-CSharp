namespace KeyAuth.Client;

using System.Security.Cryptography;
using System.Text;
using KeyAuth.Client.Models;

public sealed class LicenseValidator
{
    private readonly string _publicKey;

    public LicenseValidator(string publicKey)
    {
        _publicKey = publicKey;
    }

    public bool ValidateSignature(LicenseInfo license, string signature)
    {
        var payload = $"{license.Username}:{license.LicenseKey}:{license.ExpiresAt:O}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var signatureBytes = Convert.FromBase64String(signature);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(_publicKey);
        return rsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool IsExpired(LicenseInfo license)
    {
        return license.ExpiresAt < DateTime.UtcNow;
    }

    public bool ValidateHwid(LicenseInfo license, string currentHwid)
    {
        return string.Equals(license.BoundHwid, currentHwid, StringComparison.OrdinalIgnoreCase);
    }

    public ValidationResult FullValidation(LicenseInfo license, string signature, string currentHwid)
    {
        if (IsExpired(license))
            return new ValidationResult(false, "License has expired");

        if (!ValidateHwid(license, currentHwid))
            return new ValidationResult(false, "HWID mismatch detected");

        if (!ValidateSignature(license, signature))
            return new ValidationResult(false, "Invalid license signature");

        return new ValidationResult(true, "License valid");
    }
}

public readonly record struct ValidationResult(bool IsValid, string Message);
