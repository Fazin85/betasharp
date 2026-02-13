using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class CloseScreenS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(CloseScreenS2CPacket).TypeHandle);

    public int windowId;

    public CloseScreenS2CPacket()
    {
    }

    public CloseScreenS2CPacket(int windowId)
    {
        this.windowId = windowId;
    }

    public override void apply(NetHandler handler)
    {
        handler.onCloseScreen(this);
    }

    public override void read(DataInputStream stream)
    {
        windowId = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeByte(windowId);
    }

    public override int size()
    {
        return 1;
    }
}