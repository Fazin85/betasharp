namespace BetaSharp.Util;

public class ChunkMeshVersion
{
    private static readonly Stack<ChunkMeshVersion> s_pool = new();
    
    private long _epoch;
    private long _lastMeshed;
    private long _pendingMesh = -1;

    private ChunkMeshVersion() 
    {
        TotalAllocated++;
    }

    public static int TotalAllocated { get; private set; }
    public static int TotalReleased { get; private set; }

    public static ChunkMeshVersion Get()
    {
        if (s_pool.TryPop(out ChunkMeshVersion? version))
        {
            version._epoch = 0;
            version._lastMeshed = 0;
            version._pendingMesh = -1;
            return version;
        }
        return new ChunkMeshVersion();
    }

    public void Release()
    {
        TotalReleased++;
        s_pool.Push(this);
    }

    public void MarkDirty()
    {
        _epoch++;
    }

    public long? SnapshotIfNeeded()
    {
        if (_epoch != _lastMeshed && _pendingMesh == -1)
        {
            _pendingMesh = _epoch;
            return _epoch;
        }
        return null;
    }

    public void CompleteMesh(long snapshotEpoch)
    {
        if (_pendingMesh == snapshotEpoch)
        {
            _pendingMesh = -1;

            if (_epoch == snapshotEpoch)
            {
                _lastMeshed = snapshotEpoch;
            }
        }
    }

    public bool IsStale(long snapshotEpoch)
    {
        return _epoch > snapshotEpoch;
    }

    public bool IsModified()
    {
        return _epoch != _lastMeshed;
    }
}
