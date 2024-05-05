using System.Runtime.InteropServices;
using System.Security;

namespace Tapo.Application.Util;
public static class TapoUtils
{
    private const string TapoPlugDeviceType = "SMART.TAPOPLUG";
    private const string TapoBulbDeviceType = "SMART.TAPOBULB";
    private const string TapoIpCameraDeviceType = "SMART.IPCAMERA";
    
    public static bool IsTapoDevice(string deviceType)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceType));

        var deviceName = deviceType.ToUpper();

        return deviceName switch 
        {
            TapoPlugDeviceType => true,
            TapoBulbDeviceType => true,
            TapoIpCameraDeviceType => true,
            _ => false
        };
    }

    private static string FormatMacAddress(string text)
    {
        ArgumentNullException.ThrowIfNull(nameof(text));

        if (text.Length == 12)
        {
            return text.Insert(10, "-")
                .Insert(8, "-")
                .Insert(6, "-")
                .Insert(4, "-")
                .Insert(2, "-")
                .ToLower();
        }
        else
        {
            return text.ToLower();
        }
    }

    public static bool? TryGetIpAddressByMacAddress(string macAddress, out string? ipAddress)
    {
        var result = GetIpAddressByMacAddress(macAddress);

        if (result != null)
        {
            ipAddress = result;
            return true;
        }
        else
        {
            ipAddress = null;
            return false;
        }
    }

    public static string? GetIpAddressByMacAddress(string macAddress)
    {
        ArgumentNullException.ThrowIfNull(nameof(macAddress));

        System.Diagnostics.Process pProcess = new();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
        }
        else
        {
            pProcess.StartInfo.FileName = "arp-scan";
            pProcess.StartInfo.Arguments = "--localnet ";
        }

        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.RedirectStandardOutput = true;
        pProcess.StartInfo.CreateNoWindow = true;
        pProcess.Start();

        string strOutput = pProcess.StandardOutput.ReadToEnd();
        string[] lines = strOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        char[] sep = new[]{'\t', ' '};

        foreach (string line in lines)
        {
            string[] parts = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
       
            if (parts.Length > 1 && parts[1] == macAddress)
            {
                return parts[0];
            }
        }

        return null;
    }
}
