using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BetaSharp.Launcher.Features.Shell;
using BetaSharp.Launcher.Features.Splash;
using Microsoft.Extensions.DependencyInjection;

namespace BetaSharp.Launcher;

internal sealed class App : Application
{
    public static string Folder { get; }

    private readonly IServiceProvider _services = Bootstrapper.Build();

    static App()
    {
        string path = ResolveAppDataPath(nameof(BetaSharp));
        Folder = Path.Combine(path, "launcher");
        Directory.CreateDirectory(Folder);
    }

    private static string ResolveAppDataPath(string appName)
    {
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(home))
        {
            home = ".";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "." + appName);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Path.Combine(home, "Library", "Application Support", appName);
        }

        string? xdg = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
        return !string.IsNullOrEmpty(xdg)
            ? Path.Combine(xdg, appName)
            : Path.Combine(home, ".local", "share", appName);
    }

    public override void Initialize()
    {
        DataTemplates.Add(_services.GetRequiredService<ViewLocator>());
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _services
                .GetRequiredService<NavigationService>()
                .Navigate<SplashViewModel>();

            desktop.MainWindow = _services.GetRequiredService<ShellView>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
