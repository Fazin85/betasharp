using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerInteractEntityC2SPacket : Packet
{
    public int playerId;
    public int entityId;
    public int isLeftClick;

    public PlayerInteractEntityC2SPacket()
    {
    }

    public PlayerInteractEntityC2SPacket(int playerId, int entityId, int isLeftClick)
    {
        this.playerId = playerId;
        this.entityId = entityId;
        this.isLeftClick = isLeftClick;
    }

    public override void Read(DataInputStream stream)
    {
        playerId = stream.readInt();
        entityId = stream.readInt();
        isLeftClick = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(playerId);
        stream.writeInt(entityId);
        stream.writeByte(isLeftClick);
    }

    public override void Apply(NetHandler handler)
    {
        handler.handleInteractEntity(this);
    }

    public override int Size()
    {
        return 9;
    }
}