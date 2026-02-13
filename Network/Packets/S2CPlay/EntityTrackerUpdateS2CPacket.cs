using java.io;
using java.util;

namespace betareborn.Network.Packets.S2CPlay
{
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

        public override void read(DataInputStream var1)
        {
            id = var1.readInt();
            trackedValues = DataWatcher.readWatchableObjects(var1);
        }

        public override void write(DataOutputStream var1)
        {
            var1.writeInt(id);
            DataWatcher.writeObjectsInListToStream(trackedValues, var1);
        }

        public override void apply(NetHandler var1)
        {
            var1.onEntityTrackerUpdate(this);
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

}