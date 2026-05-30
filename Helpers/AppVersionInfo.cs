using System.Reflection;
using System.Runtime.InteropServices;

namespace MedicalCallServer.Helpers;

public static class AppVersionInfo
{
    private static readonly Version V = Assembly.GetExecutingAssembly().GetName().Version ?? new(2025, 5, 0, 0);

    public static string GetMajorVersion() => $"{V.Major}.{V.Minor}";

    public static string GetBuildVersion() => $"{V.Build}.{V.Revision}";

    public static string GetPlatformInfo() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Unknown";
}
