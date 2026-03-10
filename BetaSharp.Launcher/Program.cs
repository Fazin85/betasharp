using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.X11;
using BetaSharp.Launcher;
using Serilog;

AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
{
    if (eventArgs.ExceptionObject is TaskCanceledException exception
        && IsKnownDbusShutdownCancellation(exception))
    {
        Log.Warning(exception, "Ignoring DBus cancellation during launcher shutdown");
        Environment.Exit(0);
    }
};

TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
{
    if (IsKnownDbusShutdownCancellation(eventArgs.Exception))
    {
        Log.Warning(eventArgs.Exception, "Ignoring unobserved DBus cancellation during launcher shutdown");
        eventArgs.SetObserved();
    }
};

try
{
    Start(args);
}
catch (Exception exception)
{
    Log.Fatal(exception, "An unhandled exception occurred");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

return;

[STAThread]
static void Start(string[] args)
{
    AppBuilder
        .Configure<App>()
        .UsePlatformDetect()
        .With(new X11PlatformOptions
        {
            UseDBusMenu = false,
            UseDBusFilePicker = false,
            EnableIme = false
        })
        .WithInterFont()
        .LogToTrace()
        .StartWithClassicDesktopLifetime(args);
}

static bool IsKnownDbusShutdownCancellation(Exception exception)
{
    return exception is TaskCanceledException
           && exception.StackTrace?.Contains("Tmds.DBus.Protocol", StringComparison.Ordinal) == true;
}
