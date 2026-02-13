namespace BetaSharp.Util;

public class ObjectPool<T> : IDisposable where T : class
{
    private readonly Func<T> factory;
    private readonly PooledQueue<T> pool;
    private readonly int capacity;

    public ObjectPool(Func<T> factory, int capacity = 32)
    {
        this.factory = factory;
        this.capacity = capacity;
        pool = new PooledQueue<T>(capacity);
    }

    public T Get() => pool.IsEmpty ? factory() : pool.Dequeue();

    public void Return(T obj)
    {
        if (pool.Count < capacity)
        {
            pool.Enqueue(obj);
        }
        else if (obj is IDisposable d)
        {
            d.Dispose();
        }
    }

    public void Dispose()
    {
        while (!pool.IsEmpty)
        {
            if (pool.Dequeue() is IDisposable d) d.Dispose();
        }
    }
}