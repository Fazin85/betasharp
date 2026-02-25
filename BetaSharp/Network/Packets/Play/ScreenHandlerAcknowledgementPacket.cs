using java.io;

namespace BetaSharp.Network.Packets.Play;

public class ScreenHandlerAcknowledgementPacket : Packet
{
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

    public override void Apply(NetHandler handler)
    {
        handler.onScreenHandlerAcknowledgement(this);
    }

    public override void Read(DataInputStream stream)
    {
        syncId = (sbyte)stream.readByte();
        actionType = stream.readShort();
        accepted = (sbyte)stream.readByte() != 0;
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeByte(syncId);
        stream.writeShort(actionType);
        stream.writeByte(accepted ? 1 : 0);
    }

    public override int Size()
    {
        return 4;
    }
}