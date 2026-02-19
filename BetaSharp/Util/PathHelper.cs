using System.Runtime.InteropServices;

namespace BetaSharp.Util;

public static class PathHelper
{
    private static readonly bool s_isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private static readonly bool s_isMacOs = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static string GetAppDir(string appName)
    {
        string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userHome))
            userHome = ".";

        string path;
        if (s_isWindows)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = System.IO.Path.Combine(appData, "." + appName);
        }
        else if (s_isMacOs)
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
