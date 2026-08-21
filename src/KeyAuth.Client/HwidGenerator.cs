namespace KeyAuth.Client;

using System.Security.Cryptography;
using System.Text;

public sealed class HwidGenerator
{
    public string Generate()
    {
        var components = new StringBuilder();
        components.Append(GetProcessorId());
        components.Append(GetMotherboardSerial());
        components.Append(GetDiskSerial());
        components.Append(Environment.MachineName);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(components.ToString()));
        return Convert.ToHexString(bytes)[..32];
    }

    private static string GetProcessorId()
    {
        try
        {
            using var searcher = new System.Management.ManagementObjectSearcher(
                "SELECT ProcessorId FROM Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                return obj["ProcessorId"]?.ToString() ?? string.Empty;
            }
        }
        catch
        {
            return Environment.ProcessorCount.ToString();
        }
        return string.Empty;
    }

    private static string GetMotherboardSerial()
    {
        try
        {
            using var searcher = new System.Management.ManagementObjectSearcher(
                "SELECT SerialNumber FROM Win32_BaseBoard");
            foreach (var obj in searcher.Get())
            {
                return obj["SerialNumber"]?.ToString() ?? string.Empty;
            }
        }
        catch
        {
            return "UNKNOWN_MB";
        }
        return string.Empty;
    }

    private static string GetDiskSerial()
    {
        try
        {
            using var searcher = new System.Management.ManagementObjectSearcher(
                "SELECT SerialNumber FROM Win32_DiskDrive WHERE Index=0");
            foreach (var obj in searcher.Get())
            {
                return obj["SerialNumber"]?.ToString() ?? string.Empty;
            }
        }
        catch
        {
            return "UNKNOWN_DISK";
        }
        return string.Empty;
    }
}
