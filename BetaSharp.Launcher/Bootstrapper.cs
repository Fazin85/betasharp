using System;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.New;
using BetaSharp.Launcher.Features.New.Authentication;
using BetaSharp.Launcher.Features.Shell;
using BetaSharp.Launcher.Features.Splash;
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
            .AddTransient<SplashView>()
            .AddTransient<SplashViewModel>()
            .AddTransient<GitHubService>();

        builder
            .AddTransient<HomeView>()
            .AddTransient<HomeViewModel>();

        builder
            .AddTransient<NewView>()
            .AddTransient<NewViewModel>()
            .AddTransient<AuthenticationService>()
            .AddTransient<DownloadingService>()
            .AddTransient<LauncherService>()
            .AddTransient<MinecraftService>()
            .AddTransient<XboxService>();

        return builder.BuildServiceProvider();
    }
}
