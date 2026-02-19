using System.IO.Compression;
using Microsoft.Extensions.Logging;
using BetaSharp.Util;

namespace BetaSharp;

public record LogOptions(
    bool IsServer,
    string? BaseDirectory = null,
    bool EnableFileLogging = true
);

public static class Log
{
    private static bool _initialized;
    private static readonly object _lock = new();
    private static ILoggerFactory _factory = null!;
    private static ILogger _logger = null!;
    private static LogOptions _options = null!;

    private static ILogger Logger => _initialized ? _logger : throw new InvalidOperationException("Log.Initialize() must be called before logging.");

    public static void Debug(string? message, params object?[] args) => Logger.LogDebug(message, args);
    public static void Info(string? message, params object?[] args) => Logger.LogInformation(message, args);
    public static void Warn(string? message, params object?[] args) => Logger.LogWarning(message, args);
    public static void Error(Exception? ex = null, string? message = null, params object?[] args) => Logger.LogError(ex, message, args);
    public static void Error(string? message = null, params object?[] args) => Logger.LogError(message, args);
    public static void Fatal(Exception? ex = null, string? message = null, params object?[] args) => Logger.LogCritical(ex, message, args);

    public static void Initialize(LogOptions options)
    {
        lock (_lock)
        {
            if (_initialized)
                return;

            _options = options;

            var builder = LoggerFactory.Create(b => b
                .SetMinimumLevel(LogLevel.Debug)
                .AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                    options.SingleLine = true;
                }));

            if (options.EnableFileLogging)
            {
                string baseDir = options.BaseDirectory ?? (options.IsServer
                    ? Directory.GetCurrentDirectory()
                    : PathHelper.GetAppDir("BetaSharp"));
                string logsDir = System.IO.Path.Combine(baseDir, "logs");

                ArchiveLatestLog(logsDir);
                Directory.CreateDirectory(logsDir);
                Directory.CreateDirectory(System.IO.Path.Combine(logsDir, "crash-reports"));

                string latestPath = System.IO.Path.Combine(logsDir, "latest.log");
                builder.AddProvider(new FileLoggerProvider(latestPath));
            }

            _factory = builder;
            _logger = _factory.CreateLogger("BetaSharp");
            _initialized = true;
        }
    }


    public static void AddCrashHandlers()
    {
        if (!_initialized) throw new InvalidOperationException("Log.Initialize() must be called before AddCrashHandlers.");

        string suffix = _options.IsServer ? "server" : "client";

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            try
            {
                string baseDir = _options.BaseDirectory ?? (_options.IsServer
                    ? Directory.GetCurrentDirectory()
                    : PathHelper.GetAppDir("BetaSharp"));
                string crashDir = System.IO.Path.Combine(baseDir, "logs", "crash-reports");
                Directory.CreateDirectory(crashDir);
                string fileName = $"crash-{DateTime.Now:yyyy-MM-dd_HH.mm.ss}-{suffix}.txt";
                string path = System.IO.Path.Combine(crashDir, fileName);

                var ex = (Exception)e.ExceptionObject;
                string content = ex.ToString() + "\n\n--- Environment ---\n" +
                    $"OS: {Environment.OSVersion}\n" +
                    $"Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}";
                File.WriteAllText(path, content);
            }
            catch
            {

            }
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            try
            {
                string baseDir = _options.BaseDirectory ?? (_options.IsServer
                    ? Directory.GetCurrentDirectory()
                    : PathHelper.GetAppDir("BetaSharp"));
                string crashDir = System.IO.Path.Combine(baseDir, "logs", "crash-reports");
                Directory.CreateDirectory(crashDir);
                string fileName = $"crash-{DateTime.Now:yyyy-MM-dd_HH.mm.ss}-{suffix}-task.txt";
                string path = System.IO.Path.Combine(crashDir, fileName);

                string content = string.Join("\n---\n", e.Exception.InnerExceptions.Select(x => x.ToString()))
                    + "\n\n--- Environment ---\n"
                    + $"OS: {Environment.OSVersion}\n"
                    + $"Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}";
                File.WriteAllText(path, content);
                e.SetObserved();
            }
            catch
            {
                e.SetObserved();
            }
        };
    }

    private static void ArchiveLatestLog(string logsDir)
    {
        string latestPath = System.IO.Path.Combine(logsDir, "latest.log");
        if (!File.Exists(latestPath))
            return;

        try
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            int n = GetNextSessionNumber(logsDir, date);
            string archiveName = $"{date}-{n}.log.gz";
            string archivePath = System.IO.Path.Combine(logsDir, archiveName);

            using (var input = File.OpenRead(latestPath))
            using (var output = File.Create(archivePath))
            using (var gzip = new GZipStream(output, CompressionLevel.SmallestSize))
            {
                input.CopyTo(gzip);
            }

            File.Delete(latestPath);
        }
        catch
        {

        }
    }

    private static int GetNextSessionNumber(string logsDir, string date)
    {
        if (!Directory.Exists(logsDir))
            return 1;

        int count = Directory.GetFiles(logsDir, $"{date}-*.log.gz").Length;
        return count + 1;
    }

    public static ILogger GetLogger(Type type)
    {
        if (!_initialized)
            throw new InvalidOperationException("Log.Initialize() must be called before logging.");
        return _factory.CreateLogger(type);
    }

    public static ILogger GetLogger(string name)
    {
        if (!_initialized)
            throw new InvalidOperationException("Log.Initialize() must be called before logging.");
        return _factory.CreateLogger(name);
    }

    private sealed class FileLoggerProvider(string path) : ILoggerProvider
    {
        private readonly StreamWriter _writer = new(path, append: true) { AutoFlush = true };

        public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _writer);

        public void Dispose() => _writer.Dispose();
    }

    private sealed class FileLogger(string category, StreamWriter writer) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var msg = formatter(state, exception);
            var level = logLevel.ToString();
            lock (writer)
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {category}: {msg}");
                if (exception != null)
                    writer.WriteLine(exception.ToString());
            }
        }
    }
}
