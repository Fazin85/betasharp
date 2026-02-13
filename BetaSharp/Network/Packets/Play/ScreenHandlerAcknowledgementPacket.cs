using java.io;

namespace BetaSharp.Network.Packets.Play;

public class ScreenHandlerAcknowledgementPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ScreenHandlerAcknowledgementPacket).TypeHandle);

    public int syncId;
    public short actionType;
    public bool accepted;

    public ScreenHandlerAcknowledgementPacket()
    {
    }

    public ScreenHandlerAcknowledgementPacket(int syncId, short actionType, bool accepted)
    {
        this.syncId = syncId;
        this.actionType = actionType;
        this.accepted = accepted;
    }

    public override void apply(NetHandler handler)
    {
        handler.onScreenHandlerAcknowledgement(this);
    }

    public override void read(DataInputStream stream)
    {
        syncId = (sbyte)stream.readByte();
        actionType = stream.readShort();
        accepted = (sbyte)stream.readByte() != 0;
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeByte(syncId);
        stream.writeShort(actionType);
        stream.writeByte(accepted ? 1 : 0);
    }

    public override int size()
    {
        return 4;
    }
}