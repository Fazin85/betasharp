using System.Net.Sockets;

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

    public override void Read(NetworkStream stream)
    {
        dimensionId = (sbyte)stream.ReadByte();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteByte((byte)dimensionId);
    }

    public override int Size()
    {
        return 1;
    }
}
