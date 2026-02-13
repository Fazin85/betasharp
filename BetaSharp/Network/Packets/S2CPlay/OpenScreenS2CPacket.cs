using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class OpenScreenS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(OpenScreenS2CPacket).TypeHandle);

    public int syncId;
    public int screenHandlerId;
    public string name;
    public int slotsCount;

    public OpenScreenS2CPacket()
    {
    }

    public OpenScreenS2CPacket(int syncId, int screenHandlerId, String name, int size)
    {
        this.syncId = syncId;
        this.screenHandlerId = screenHandlerId;
        this.name = name;
        slotsCount = size;
    }

    public override void apply(NetHandler handler)
    {
        handler.onOpenScreen(this);
    }

    public override void read(DataInputStream stream)
    {
        syncId = (sbyte)stream.readByte();
        screenHandlerId = (sbyte)stream.readByte();
        name = stream.readUTF();
        slotsCount = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeByte(syncId);
        stream.writeByte(screenHandlerId);
        stream.writeUTF(name);
        stream.writeByte(slotsCount);
    }

    public override int size()
    {
        return 3 + name.Length;
    }
}