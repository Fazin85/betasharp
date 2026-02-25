using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityVehicleSetS2CPacket : Packet
{
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

    public override int Size()
    {
        return 8;
    }

    public override void Read(DataInputStream stream)
    {
        entityId = stream.readInt();
        vehicleEntityId = stream.readInt();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeInt(vehicleEntityId);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityVehicleSet(this);
    }
}