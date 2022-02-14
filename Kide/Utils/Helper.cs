using Avalonia.Platform;

namespace Kide.Utils
{
    public static class Helper
    {
        public static string GetOsOutputExt() => App.Platform == OperatingSystemType.WinNT ? ".exe" : "";
    }
}