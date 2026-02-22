using System;
using System.IO;
using BetaSharp.Launcher.Features;
using BetaSharp.Launcher.Features.Authentication;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.Mojang;
using BetaSharp.Launcher.Features.Shell;
using BetaSharp.Launcher.Features.Splash;
using CommunityToolkit.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BetaSharp.Launcher;

internal static partial class Bootstrapper
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddHttpClient<DownloadingService>();
        services.AddHttpClient<XboxService>();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            // Find a way to display class names and hide HttpClient's logs.
            const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    Path.Combine(App.Folder, "Logs", ".txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 5,
                    outputTemplate: template)
                .CreateLogger();

            builder.AddSerilog(Log.Logger);
        });

        ConfigureServices(services);

        return services.BuildServiceProvider();
    }

    [Singleton(typeof(ViewLocator))]
    [Singleton(typeof(AccountService))]
    [Singleton(typeof(AuthenticationService))]
    [Transient(typeof(MojangClient))]
    [Transient(typeof(DownloadingService))]
    [Transient(typeof(AuthenticationView))]
    [Transient(typeof(AuthenticationViewModel))]
    [Transient(typeof(HomeView))]
    [Transient(typeof(HomeViewModel))]
    [Transient(typeof(ShellView))]
    [Transient(typeof(ShellViewModel))]
    [Transient(typeof(SplashView))]
    [Transient(typeof(SplashViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);
}
