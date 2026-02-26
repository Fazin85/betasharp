using System.Net.Sockets;
using BetaSharp.Entities;

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

    public override void Read(NetworkStream stream)
    {
        entityId = stream.ReadInt();
        vehicleEntityId = stream.ReadInt();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteInt(vehicleEntityId);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityVehicleSet(this);
    }
}
