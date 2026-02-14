using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace BetaSharp.Launcher.Features.New;

internal sealed class LauncherService
{
    private readonly Window? window = ((ClassicDesktopStyleApplicationLifetime?) Application.Current?.ApplicationLifetime)?.MainWindow;

    public async Task LaunchAsync(string uri)
    {
        ArgumentNullException.ThrowIfNull(window);
        await window.Launcher.LaunchUriAsync(new Uri(uri));
    }
}