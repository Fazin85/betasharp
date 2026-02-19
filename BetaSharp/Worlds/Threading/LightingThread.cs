using System.Collections.Concurrent;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks.Light;

namespace BetaSharp.Worlds.Threading;

// Dedicated thread for processing lighting updates asynchronously.
// Uses a concurrent queue for thread-safe operations and implements a budget cap
// to prevent tick stalls.

// Thread Safety:
// - Uses ConcurrentQueue for thread-safe enqueue/dequeue operations
// - The lighting lock in World.cs must be acquired before modifying chunk light data
// - Volatile flag used for shutdown signaling
public class LightingThread
{
    private readonly World _world;
    private readonly Thread _thread;
    private readonly ConcurrentQueue<LightUpdate> _queue = new();
    private volatile bool _running = true;
    
    // Budget cap to prevent "can't keep up" errors - process at most this many updates per tick
    private const int MAX_UPDATES_PER_TICK = 50;
    // Maximum queue size before we start dropping updates
    private const int MAX_QUEUE_SIZE = 500;

    public LightingThread(World world)
    {
        _world = world;
        _thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "Lighting-Thread"
        };
    }

    public void Start()
    {
        _thread.Start();
    }

    public void Stop()
    {
        _running = false;
        _thread.Join(TimeSpan.FromSeconds(5));
    }

    public void QueueLightUpdate(LightUpdate update)
    {
        // Don't queue if we're shutting down
        if (!_running) return;
        
        // Don't let the queue grow unbounded - if it's too full, skip this update
        if (_queue.Count >= MAX_QUEUE_SIZE)
        {
            return;
        }
        
        _queue.Enqueue(update);
    }

    public int GetQueueSize()
    {
        return _queue.Count;
    }

    // Process a single lighting update from the queue.
    // Called by the main thread during spawn prep to drain the async queue.
    public void ProcessOneUpdate()
    {
        if (_queue.TryDequeue(out LightUpdate update))
        {
            try
            {
                lock (_world.LightingLock)
                {
                    update.UpdateLight(_world);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error processing light update: {ex.Message}");
            }
        }
    }

    private void Run()
    {
        while (_running)
        {
            try
            {
                ProcessLightingUpdates();
                
                // Sleep briefly to avoid spinning
                Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Log.Error($"Lighting thread error: {ex.Message}");
            }
        }
    }

    private void ProcessLightingUpdates()
    {
        int processed = 0;
        
        // Process up to MAX_UPDATES_PER_TICK lighting updates per call
        // This prevents the lighting thread from consuming too much CPU
        while (processed < MAX_UPDATES_PER_TICK && _queue.TryDequeue(out LightUpdate update))
        {
            try
            {
                // Update lighting for this update
                // Thread safety is ensured by acquiring the lighting lock in World.cs
                lock (_world.LightingLock)
                {
                    update.UpdateLight(_world);
                }
                processed++;
            }
            catch (Exception ex)
            {
                Log.Error($"Error processing light update: {ex.Message}");
            }
        }
    }
}
