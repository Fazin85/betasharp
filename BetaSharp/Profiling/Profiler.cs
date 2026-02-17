using System.Collections.Concurrent;
using System.Diagnostics;

namespace BetaSharp.Profiling;

public static class Profiler
{
    public const int HistoryLength = 300;
    public static bool Enabled = false;

    private class ProfilerData
    {
        public required string Name;
        public double LastExecutionTime;
        public double AverageExecutionTime;

        public double CurrentPeriodMax;
        public double PreviousPeriodMax;

        public double[] History = new double[HistoryLength];
        public int HistoryIndex;

        public Stopwatch Stopwatch = new();

        public void Update(double time)
        {
            LastExecutionTime = time;

            if (AverageExecutionTime == 0) AverageExecutionTime = time;
            else AverageExecutionTime = AverageExecutionTime * 0.95 + time * 0.05;

            if (time > CurrentPeriodMax) CurrentPeriodMax = time;
        }
    }

    private class ThreadProfiler(string name)
    {
        public readonly string ThreadName = name;
        public readonly ConcurrentDictionary<string, ProfilerData> Sections = new();
        public readonly Stack<string> GroupStack = new();

        public string GetCurrentPath(string name)
        {
            if (GroupStack.Count == 0) return name;

            string[] arr = [.. GroupStack];
            Array.Reverse(arr);
            return string.Join("/", arr) + "/" + name;
        }
    }

    private static readonly ConcurrentBag<ThreadProfiler> s_allProfilers = [];

    private static readonly ThreadLocal<ThreadProfiler> s_localProfiler = new(() =>
    {
        Thread thread = Thread.CurrentThread;
        string name = !string.IsNullOrEmpty(thread.Name) ? thread.Name
                 : $"Thread-{thread.ManagedThreadId}";
        var p = new ThreadProfiler(name);
        s_allProfilers.Add(p);
        return p;
    });

    private static double s_maxResetTimer;

    public static void PushGroup(string name)
    {
        if (!Enabled) return;
        s_localProfiler.Value!.GroupStack.Push(name);
    }

    public static void PopGroup()
    {
        Stack<string> stack = s_localProfiler.Value!.GroupStack;
        if (stack.Count > 0)
        {
            stack.Pop();
        }
    }

    public static void Start(string name)
    {
        if (!Enabled) return;
        ThreadProfiler profiler = s_localProfiler.Value!;
        string fullName = profiler.GetCurrentPath(name);
        ProfilerData data = profiler.Sections.GetOrAdd(fullName, n => new ProfilerData { Name = n });
        data.Stopwatch.Restart();
    }

    public static void Stop(string name)
    {
        if (!Enabled) return;
        ThreadProfiler profiler = s_localProfiler.Value!;
        string fullName = profiler.GetCurrentPath(name);
        if (profiler.Sections.TryGetValue(fullName, out ProfilerData? data))
        {
            data.Stopwatch.Stop();
            data.Update(data.Stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    public static void Record(string name, double milliseconds)
    {
        if (!Enabled) return;
        ThreadProfiler profiler = s_localProfiler.Value!;
        string fullName = profiler.GetCurrentPath(name);
        ProfilerData data = profiler.Sections.GetOrAdd(fullName, n => new ProfilerData { Name = n });
        data.Update(milliseconds);
    }

    public static void Update(double dt)
    {
        if (!Enabled) return;
        s_maxResetTimer += dt;
        if (s_maxResetTimer >= 1.0)
        {
            s_maxResetTimer = 0;

            foreach (ThreadProfiler profiler in s_allProfilers)
            {
                foreach (ProfilerData section in profiler.Sections.Values)
                {
                    section.PreviousPeriodMax = section.CurrentPeriodMax;
                    section.CurrentPeriodMax = 0;
                }
            }
        }
    }

    public static void CaptureFrame()
    {
        if (!Enabled) return;

        foreach (ThreadProfiler profiler in s_allProfilers)
        {
            foreach (ProfilerData section in profiler.Sections.Values)
            {
                section.History[section.HistoryIndex] = section.LastExecutionTime;
                section.HistoryIndex = (section.HistoryIndex + 1) % HistoryLength;
            }
        }
    }

    public static IEnumerable<(string Name, double Last, double Avg, double Max, double[] History, int HistoryIndex)> GetStats()
    {
        var allStats = new List<(string Name, double Last, double Avg, double Max, double[] History, int HistoryIndex)>();

        foreach (ThreadProfiler profiler in s_allProfilers)
        {
            foreach (ProfilerData x in profiler.Sections.Values)
            {
                string displayName = $"[{profiler.ThreadName}] {x.Name}";

                allStats.Add((displayName, x.LastExecutionTime, x.AverageExecutionTime, Math.Max(x.CurrentPeriodMax, x.PreviousPeriodMax), x.History, x.HistoryIndex));
            }
        }

        return allStats.OrderBy(x => x.Name);
    }
}
