using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace BetaSharp.Launcher;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new LauncherWindow();

            // Set shutdown mode
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        base.OnFrameworkInitializationCompleted();
    }
}