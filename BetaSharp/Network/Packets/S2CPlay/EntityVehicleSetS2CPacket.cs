using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityVehicleSetS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityVehicleSetS2CPacket).TypeHandle);

    public int entityId;
    public int vehicleEntityId;

    public EntityVehicleSetS2CPacket()
    {
    }

    public EntityVehicleSetS2CPacket(Entity entity, Entity vehicle)
    {
        entityId = entity.id;
        vehicleEntityId = vehicle != null ? vehicle.id : -1;
    }

    public override int size()
    {
        return 8;
    }

    public override void read(DataInputStream stream)
    {
        entityId = stream.readInt();
        vehicleEntityId = stream.readInt();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeInt(vehicleEntityId);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityVehicleSet(this);
    }
}