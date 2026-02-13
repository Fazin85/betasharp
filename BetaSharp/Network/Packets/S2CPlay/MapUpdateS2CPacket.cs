using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class MapUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(MapUpdateS2CPacket).TypeHandle);

    public short itemRawId;
    public short id;
    public byte[] updateData;

    public MapUpdateS2CPacket()
    {
        worldPacket = true;
    }

    public MapUpdateS2CPacket(short itemRawId, short id, byte[] updateData)
    {
        worldPacket = true;
        this.itemRawId = itemRawId;
        this.id = id;
        this.updateData = updateData;
    }

    public override void read(DataInputStream stream)
    {
        itemRawId = stream.readShort();
        id = stream.readShort();
        updateData = new byte[(sbyte)stream.readByte() & 255];
        stream.readFully(updateData);
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeShort(itemRawId);
        stream.writeShort(id);
        stream.writeByte(updateData.Length);
        stream.write(updateData);
    }

    public override void apply(NetHandler handler)
    {
        handler.onMapUpdate(this);
    }

    public override int size()
    {
        return 4 + updateData.Length;
    }
}