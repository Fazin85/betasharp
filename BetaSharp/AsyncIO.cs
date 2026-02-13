namespace BetaSharp;

public class AsyncIO
{
    private static readonly List<Task> tasks = [];
    private static readonly object l = new();

    public static void addTask(Task task)
    {
        lock (l)
        {
            tasks.Add(task);
        }
    }

    public static void tick()
    {
        lock (l)
        {
            tasks.RemoveAll(x => x.IsCompleted);
        }
    }

    public static int activeTaskCount()
    {
        lock (l)
        {
            return tasks.Where(x => !x.IsCompleted).Count();
        }
    }

    public static bool isBlocked()
    {
        lock (l)
        {
            tasks.RemoveAll(x => x.IsCompleted);
            return tasks.Any(x => !x.IsCompleted);
        }
    }
}