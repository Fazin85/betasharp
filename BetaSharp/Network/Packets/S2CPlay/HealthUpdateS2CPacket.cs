using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class HealthUpdateS2CPacket : Packet
{
    public int healthMP;

    public HealthUpdateS2CPacket()
    {
    }

    public HealthUpdateS2CPacket(int health)
    {
        healthMP = health;
    }

    public override void Read(DataInputStream stream)
    {
        healthMP = stream.readShort();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeShort(healthMP);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onHealthUpdate(this);
    }

    public override int Size()
    {
        return 2;
    }
}