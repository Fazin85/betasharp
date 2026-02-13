using System.Collections.Concurrent;
using System.Diagnostics;

namespace BetaSharp.Profiling;

public static class Profiler
{
    public const int HistoryLength = 300;
    public static bool Enabled = false;

    private class ProfilerData
    {
        public string Name;
        public double LastExecutionTime;
        public double AverageExecutionTime;

        public double CurrentPeriodMax;
        public double PreviousPeriodMax;

        public double[] History = new double[HistoryLength];
        public int HistoryIndex = 0;

        public Stopwatch Stopwatch = new();

        public void Update(double time)
        {
            LastExecutionTime = time;

            if (AverageExecutionTime == 0) AverageExecutionTime = time;
            else AverageExecutionTime = AverageExecutionTime * 0.95 + time * 0.05;

            if (time > CurrentPeriodMax) CurrentPeriodMax = time;
        }

    }

    private static readonly ConcurrentDictionary<string, ProfilerData> _sections = new();
    private static double _maxResetTimer = 0;

    private static readonly AsyncLocal<Stack<string>> _groupStack = new();

    private static Stack<string> GetStack()
    {
        return _groupStack.Value ??= new Stack<string>();
    }

    private static string GetCurrentPath(string name)
    {
        var stack = GetStack();
        if (stack.Count == 0) return name;

        return string.Join("/", stack.Reverse()) + "/" + name;
    }

    public static void PushGroup(string name)
    {
        if (!Enabled) return;
        GetStack().Push(name);
    }

    public static void PopGroup()
    {
        var stack = GetStack();
        if (stack.Count > 0)
            stack.Pop();
    }

    public static void Start(string name)
    {
        if (!Enabled) return;
        string fullName = GetCurrentPath(name);
        var data = _sections.GetOrAdd(fullName, n => new ProfilerData { Name = n });
        data.Stopwatch.Restart();
    }

    public static void Stop(string name)
    {
        if (!Enabled) return;
        string fullName = GetCurrentPath(name);
        if (_sections.TryGetValue(fullName, out var data))
        {
            data.Stopwatch.Stop();
            data.Update(data.Stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    public static void Record(string name, double milliseconds)
    {
        if (!Enabled) return;
        string fullName = GetCurrentPath(name);
        var data = _sections.GetOrAdd(fullName, n => new ProfilerData { Name = n });
        data.Update(milliseconds);
    }


    public static void Update(double dt)
    {
        if (!Enabled) return;
        _maxResetTimer += dt;
        if (_maxResetTimer >= 1.0)
        {
            _maxResetTimer = 0;
            foreach (var section in _sections.Values)
            {
                section.PreviousPeriodMax = section.CurrentPeriodMax;
                section.CurrentPeriodMax = 0;
            }
        }
    }



    public static void CaptureFrame()
    {
        if (!Enabled) return;
        foreach (var section in _sections.Values)
        {
            section.History[section.HistoryIndex] = section.LastExecutionTime;
            section.HistoryIndex = (section.HistoryIndex + 1) % Profiler.HistoryLength;
        }

    }

    public static IEnumerable<(string Name, double Last, double Avg, double Max, double[] History, int HistoryIndex)> GetStats()
    {
        return _sections.Values.OrderBy(x => x.Name)
            .Select(x => (x.Name, x.LastExecutionTime, x.AverageExecutionTime, Math.Max(x.CurrentPeriodMax, x.PreviousPeriodMax), x.History, x.HistoryIndex));
    }
}