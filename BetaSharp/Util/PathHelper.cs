using System.Runtime.InteropServices;

namespace BetaSharp.Util;

public static class PathHelper
{
    private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private static readonly bool IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static string GetAppDir(string appName)
    {
        string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userHome))
            userHome = ".";

        string path;
        if (IsWindows)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = System.IO.Path.Combine(appData, "." + appName);
        }
        else if (IsMacOS)
        {
            path = System.IO.Path.Combine(userHome, "Library", "Application Support", appName);
        }
        else
        {
            path = System.IO.Path.Combine(userHome, "." + appName);
        }

        Directory.CreateDirectory(path);
        return path;
    }
}
