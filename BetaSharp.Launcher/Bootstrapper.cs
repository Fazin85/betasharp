using System;
using System.IO;
using BetaSharp.Launcher.Features;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.New;
using BetaSharp.Launcher.Features.New.Services;
using BetaSharp.Launcher.Features.Shell;
using BetaSharp.Launcher.Features.Splash;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BetaSharp.Launcher;

internal static class Bootstrapper
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            // Find a way to display class names and hide HttpClient's logs.
            const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} {Message:lj}{NewLine}{Exception}";

            var logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Debug(outputTemplate: template)
                .WriteTo.File(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BetaSharp Launcher", "Logs", ".txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 5,
                    outputTemplate: template)
                .CreateLogger();

            builder.AddSerilog(logger, true);
        });

        services.AddHttpClient();

        services.AddSingleton<ViewLocator>();

        services.AddSingleton<AuthenticationService>();

        services
            .AddTransient<ShellView>()
            .AddTransient<ShellViewModel>();

        services
            .AddTransient<SplashView>()
            .AddTransient<SplashViewModel>()
            .AddTransient<GitHubService>();

        services
            .AddTransient<HomeView>()
            .AddTransient<HomeViewModel>();

        services
            .AddTransient<NewView>()
            .AddTransient<NewViewModel>()
            .AddTransient<DownloadingService>()
            .AddTransient<MinecraftService>()
            .AddTransient<XboxService>();

        return services.BuildServiceProvider();
    }
}
