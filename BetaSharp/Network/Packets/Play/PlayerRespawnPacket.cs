using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerRespawnPacket : Packet
{
    public sbyte dimensionId;

    public PlayerRespawnPacket()
    {
    }

    public PlayerRespawnPacket(sbyte dimensionId)
    {
        this.dimensionId = dimensionId;
    }

    public override void Apply(NetHandler handler)
    {
        handler.onPlayerRespawn(this);
    }

    public override void Read(DataInputStream stream)
    {
        dimensionId = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeByte(dimensionId);
    }

    public override int Size()
    {
        return 1;
    }
}