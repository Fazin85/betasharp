using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BetaSharp.Launcher.Features.Shell;
using Microsoft.Extensions.DependencyInjection;

namespace BetaSharp.Launcher;

public sealed partial class App : Application
{
    private readonly IServiceProvider services = Bootstrapper.Build();

    public override void Initialize()
    {
        DataTemplates.Add(services.GetRequiredService<ViewLocator>());
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = services.GetRequiredService<ShellView>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}