namespace BetaSharp.Threading;

/// <summary>
/// A C# base class that mimics java.lang.Thread, used to replace IKVM-based threading.
/// Wraps <see cref="System.Threading.Thread"/> and exposes the Java-style API.
/// </summary>
public abstract class JavaThread
{
    private readonly Thread _thread;

    protected JavaThread(string name)
    {
        _thread = new Thread(run) { Name = name, IsBackground = true };
    }

    protected JavaThread() : this("JavaThread") { }

    public abstract void run();

    public void start() => _thread.Start();

    public bool isAlive() => _thread.IsAlive;

    public void setDaemon(bool daemon) => _thread.IsBackground = daemon;

    public void stop() => _thread.Interrupt();

    public void interrupt() => _thread.Interrupt();

    public static void sleep(long ms) => Thread.Sleep((int)ms);
}
