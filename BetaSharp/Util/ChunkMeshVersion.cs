namespace BetaSharp.Util;

public class ChunkMeshVersion
{
    private long epoch = 0;
    private long lastMeshed = 0;
    private long pendingMesh = -1;

    public void MarkDirty()
    {
        epoch++;
    }

    public long? SnapshotIfNeeded()
    {
        if (epoch != lastMeshed && pendingMesh == -1)
        {
            pendingMesh = epoch;
            return epoch;
        }
        return null;
    }

    public void CompleteMesh(long snapshotEpoch)
    {
        if (pendingMesh == snapshotEpoch)
        {
            pendingMesh = -1;

            if (epoch == snapshotEpoch)
            {
                lastMeshed = snapshotEpoch;
            }
        }
    }

    public bool IsStale(long snapshotEpoch)
    {
        return epoch > snapshotEpoch;
    }

    public bool IsModified()
    {
        return epoch != lastMeshed;
    }
}