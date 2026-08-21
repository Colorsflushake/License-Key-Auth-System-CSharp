namespace KeyAuth.Client;

using System.Diagnostics;
using System.Security.Cryptography;

public sealed class IntegrityChecker
{
    private readonly string _expectedHash;

    public IntegrityChecker(string expectedHash)
    {
        _expectedHash = expectedHash;
    }

    public bool VerifyExecutableIntegrity()
    {
        var currentPath = Environment.ProcessPath;
        if (currentPath is null) return false;

        var hash = ComputeFileHash(currentPath);
        return string.Equals(hash, _expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    public bool DetectDebugger()
    {
        if (Debugger.IsAttached) return true;

        var currentProcess = Process.GetCurrentProcess();
        var modules = currentProcess.Modules;

        string[] suspiciousModules = ["dnSpy.dll", "Harmony.dll", "de4dot.dll"];
        foreach (ProcessModule module in modules)
        {
            if (suspiciousModules.Any(s =>
                module.ModuleName.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }
        return false;
    }

    public bool VerifyMemoryIntegrity(nint baseAddress, int size, string expectedCrc)
    {
        var buffer = new byte[size];
        Buffer.BlockCopy(new byte[size], 0, buffer, 0, size);
        var crc = ComputeCrc32(buffer);
        return string.Equals(crc, expectedCrc, StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeFileHash(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

    private static string ComputeCrc32(byte[] data)
    {
        uint crc = 0xFFFFFFFF;
        foreach (var b in data)
        {
            crc ^= b;
            for (int i = 0; i < 8; i++)
            {
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
            }
        }
        return (~crc).ToString("X8");
    }
}
