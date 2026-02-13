using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ScreenHandlerPropertyUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ScreenHandlerPropertyUpdateS2CPacket).TypeHandle);

    public int syncId;
    public int propertyId;
    public int value;

    public ScreenHandlerPropertyUpdateS2CPacket()
    {
    }

    public ScreenHandlerPropertyUpdateS2CPacket(int syncId, int propertyId, int value)
    {
        this.syncId = syncId;
        this.propertyId = propertyId;
        this.value = value;
    }

    public override void apply(NetHandler handler)
    {
        handler.onScreenHandlerPropertyUpdate(this);
    }

    public override void read(DataInputStream stream)
    {
        syncId = (sbyte)stream.readByte();
        propertyId = stream.readShort();
        value = stream.readShort();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeByte(syncId);
        stream.writeShort(propertyId);
        stream.writeShort(value);
    }

    public override int size()
    {
        return 5;
    }
}