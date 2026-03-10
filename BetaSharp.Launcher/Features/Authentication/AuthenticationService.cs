using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace BetaSharp.Launcher.Features.Authentication;

internal sealed class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly SystemWebViewOptions _webViewOptions;
    private readonly IPublicClientApplication _application;

    private bool _initialized;

    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _logger = logger;

        const string success = """
                               <!DOCTYPE html><html lang="en"><head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>BetaSharp Launcher</title><link rel="icon" type="image/png" href="https://raw.githubusercontent.com/Fazin85/betasharp/refs/heads/main/BetaSharp.Launcher/logo.ico"><style>body{margin:0;padding:0;background-color:#000;display:flex;justify-content:center;align-items:center;height:100vh;font-family:Arial,sans-serif}p{color:#fff;font-size:.85rem;font-weight:400;text-align:center;opacity:.5}</style></head><body><p>You can close this tab now</p></body></html>
                               """;

        const string failure = """
                               <!DOCTYPE html><html lang="en"><head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>BetaSharp Launcher</title><link rel="icon" type="image/png" href="https://raw.githubusercontent.com/Fazin85/betasharp/refs/heads/main/BetaSharp.Launcher/logo.ico"><style>body{margin:0;padding:0;background-color:#000;display:flex;justify-content:center;align-items:center;height:100vh;font-family:Arial,sans-serif}p{color:orange;font-size:1rem;font-weight:400;text-align:center}a{color:#58a6ff;text-decoration:none}a:hover{text-decoration:underline}</style></head><body><p>Failed to authenticate please raise an issue <a href="https://github.com/Fazin85/betasharp/issues" target="_blank">here</a></p></body></html>
                               """;

        // Need better way to store the HTML responses.
        _webViewOptions = new SystemWebViewOptions
        {
            HtmlMessageSuccess = success,
            HtmlMessageError = failure
        };

        // Probably not the best idea to use Prism's ID?
        _application = PublicClientApplicationBuilder
            .Create("C36A9FB6-4F2A-41FF-90BD-AE7CC92031EB")
            .WithAuthority("https://login.microsoftonline.com/consumers")
            .WithRedirectUri("http://localhost")
            .Build();
    }

    public async Task<string> AuthenticateAsync()
    {
        if (!_initialized)
        {
            _logger.LogInformation("Initializing authentication service");

            string path = Path.Combine(App.Folder, "betasharp.launcher.cache");

            var properties = new StorageCreationPropertiesBuilder(Path.GetFileName(path), Path.GetDirectoryName(path))
                .WithLinuxKeyring(
                    "betasharp.launcher",
                    MsalCacheHelper.LinuxKeyRingDefaultCollection,
                    "MSAL cache for BetaSharp's launcher",
                    new KeyValuePair<string, string>("Version", "1"),
                    new KeyValuePair<string, string>("Application", "BetaSharp.Launcher"))
                .WithMacKeyChain("betasharp.launcher", "betasharp")
                .Build();

            try
            {
                var helper = await MsalCacheHelper.CreateAsync(properties);
                helper.RegisterCache(_application.UserTokenCache);
            }
            catch (MsalCachePersistenceException exception)
            {
                _logger.LogWarning(exception, "Unable to initialize persistent authentication cache, continuing with in-memory cache");
            }

            _initialized = true;
        }

        try
        {
            var accounts = await _application.GetAccountsAsync();

            var result = await _application
                .AcquireTokenSilent(["XboxLive.signin offline_access"], accounts.FirstOrDefault())
                .ExecuteAsync();

            _logger.LogInformation("Authenticated silently for Microsoft account");

            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            _logger.LogInformation("Authenticated interactively for Microsoft account");
            EnsureInteractiveAuthPrerequisites();

            // Find a way to use system brokers.
            var result = await _application
                .AcquireTokenInteractive(["XboxLive.signin offline_access"])
                .WithUseEmbeddedWebView(false)
                .WithSystemWebViewOptions(_webViewOptions)
                .ExecuteAsync();

            return result.AccessToken;
        }
    }

    private static void EnsureInteractiveAuthPrerequisites()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            List<string> missing = [];

            if (!IsCommandAvailable("xdg-open"))
            {
                missing.Add("The `xdg-open` command is not available.");
            }

            AddIfLibraryMissing(missing, "webkit2gtk and/or libsoup");

            if (missing.Count == 0)
            {
                return;
            }

            string message = BuildMissingDependencyMessage(
                "Microsoft sign-in is missing required Linux dependencies.",
                missing,
                "Install WebKitGTK and/or libsoup packages for your distro, then try again.");

            throw new AuthenticationPreflightException(message);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !File.Exists("/usr/bin/open"))
        {
            throw new AuthenticationPreflightException(
                "Microsoft sign-in requires the macOS `open` command at `/usr/bin/open`.");
        }
    }

    private static void AddIfLibraryMissing(ICollection<string> missing, string libraryName)
    {
        if (!TryLoadLibrary(libraryName))
        {
            missing.Add($"Missing native library `{libraryName}`.");
        }
    }

    private static bool TryLoadLibrary(string libraryName)
    {
        if (NativeLibrary.TryLoad(libraryName, out nint handle))
        {
            NativeLibrary.Free(handle);
            return true;
        }

        return false;
    }

    private static bool IsCommandAvailable(string command)
    {
        string? path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        foreach (string segment in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            string candidate = Path.Combine(segment, command);
            if (File.Exists(candidate))
            {
                return true;
            }
        }

        return false;
    }

    private static string BuildMissingDependencyMessage(string title, IEnumerable<string> missing, string help)
    {
        var builder = new StringBuilder();
        builder.AppendLine(title);

        foreach (string item in missing)
        {
            builder.Append("- ").AppendLine(item);
        }

        builder.Append(help);
        return builder.ToString();
    }
}
