using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class MapUpdateS2CPacket : Packet
{
    public short itemRawId;
    public short id;
    public byte[] updateData;

    public MapUpdateS2CPacket()
    {
        WorldPacket = true;
    }

    public MapUpdateS2CPacket(short itemRawId, short id, byte[] updateData)
    {
        WorldPacket = true;
        this.itemRawId = itemRawId;
        this.id = id;
        this.updateData = updateData;
    }

    public override void Read(DataInputStream stream)
    {
        itemRawId = stream.readShort();
        id = stream.readShort();
        updateData = new byte[(sbyte)stream.readByte() & 255];
        stream.readFully(updateData);
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeShort(itemRawId);
        stream.writeShort(id);
        stream.writeByte(updateData.Length);
        stream.write(updateData);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onMapUpdate(this);
    }

    public override int Size()
    {
        return 4 + updateData.Length;
    }
}