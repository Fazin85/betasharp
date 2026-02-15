using System;
using BetaSharp.Launcher.Features.New;
using BetaSharp.Launcher.Features.Shell;
using BetaSharp.Launcher.Features.Splash;
using Microsoft.Extensions.DependencyInjection;

namespace BetaSharp.Launcher;

internal static class Bootstrapper
{
    public static IServiceProvider Build()
    {
        var builder = new ServiceCollection();

        builder.AddSingleton<ViewLocator>();

        builder
            .AddTransient<ShellView>()
            .AddTransient<ShellViewModel>();

        builder
            .AddTransient<SplashView>()
            .AddTransient<SplashViewModel>();

        builder
            .AddTransient<NewView>()
            .AddTransient<NewViewModel>()
            .AddTransient<AuthenticationService>()
            .AddTransient<DownloadingService>()
            .AddHttpClient();

        return builder.BuildServiceProvider();
    }
}