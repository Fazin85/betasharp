using Avalonia;
using System;
using BetaSharp.Launcher;
using Serilog;

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
        .WithInterFont()
        .LogToTrace()
        .StartWithClassicDesktopLifetime(args);
}
