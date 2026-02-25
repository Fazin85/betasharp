using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class CloseScreenS2CPacket : Packet
{
    public int windowId;

    public CloseScreenS2CPacket()
    {
    }

    public CloseScreenS2CPacket(int windowId)
    {
        this.windowId = windowId;
    }

    public override void Apply(NetHandler handler)
    {
        handler.onCloseScreen(this);
    }

    public override void Read(DataInputStream stream)
    {
        windowId = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeByte(windowId);
    }

    public override int Size()
    {
        return 1;
    }
}