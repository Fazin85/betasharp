using java.io;
using java.util;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityTrackerUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityTrackerUpdateS2CPacket).TypeHandle);

    public int id;
    private List trackedValues;

    public EntityTrackerUpdateS2CPacket()
    {
    }

    public EntityTrackerUpdateS2CPacket(int entityId, DataWatcher dataWatcher)
    {
        id = entityId;
        trackedValues = dataWatcher.getDirtyEntries();
    }

    public override void read(DataInputStream stream)
    {
        id = stream.readInt();
        trackedValues = DataWatcher.readWatchableObjects(stream);
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(id);
        DataWatcher.writeObjectsInListToStream(trackedValues, stream);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityTrackerUpdate(this);
    }

    public override int size()
    {
        return 5;
    }

    public List getWatchedObjects()
    {
        return trackedValues;
    }
}