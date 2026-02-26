using System.Net.Sockets;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityTrackerUpdateS2CPacket : Packet
{
    public int id;
    private List<WatchableObject> trackedValues;

    public EntityTrackerUpdateS2CPacket() { }

    public EntityTrackerUpdateS2CPacket(int entityId, DataWatcher dataWatcher)
    {
        id = entityId;
        trackedValues = dataWatcher.GetDirtyEntries();
    }

    public override void Read(NetworkStream stream)
    {
        id = stream.ReadInt();
        trackedValues = DataWatcher.ReadWatchableObjects(stream);
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(id);
        DataWatcher.WriteObjectsInListToStream(trackedValues, stream);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityTrackerUpdate(this);
    }

    public override int Size()
    {
        return 5;
    }

    public List<WatchableObject> GetWatchedObjects()
    {
        return trackedValues;
    }
}
