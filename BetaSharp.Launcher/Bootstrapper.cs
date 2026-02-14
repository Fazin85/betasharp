using System;
using BetaSharp.Launcher.Features.New;
using BetaSharp.Launcher.Features.Shell;
using Microsoft.Extensions.DependencyInjection;

namespace BetaSharp.Launcher;

internal static class Bootstrapper
{
    public static IServiceProvider Build()
    {
        var builder = new ServiceCollection();

        builder.AddHttpClient();
        builder.AddSingleton<ViewLocator>();
        
        builder
            .AddTransient<ShellView>()
            .AddTransient<ShellViewModel>();

        builder
            .AddTransient<NewView>()
            .AddTransient<NewViewModel>()
            .AddTransient<AuthenticationService>()
            .AddTransient<LauncherService>();

        return builder.BuildServiceProvider();
    }
}