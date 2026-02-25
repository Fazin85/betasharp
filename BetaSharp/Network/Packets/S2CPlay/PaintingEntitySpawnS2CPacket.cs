using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PaintingEntitySpawnS2CPacket : Packet
{
    public int entityId;
    public int xPosition;
    public int yPosition;
    public int zPosition;
    public int direction;
    public string title;

    public PaintingEntitySpawnS2CPacket()
    {
    }

    public PaintingEntitySpawnS2CPacket(EntityPainting paint)
    {
        entityId = paint.id;
        xPosition = paint.XPosition;
        yPosition = paint.YPosition;
        zPosition = paint.ZPosition;
        direction = paint.Direction;
        title = paint.Art.Title;
    }

    public override void Read(DataInputStream stream)
    {
        entityId = stream.readInt();
        title = ReadString(stream, EnumArt.MaxArtTitleLength);
        xPosition = stream.readInt();
        yPosition = stream.readInt();
        zPosition = stream.readInt();
        direction = stream.readInt();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        WriteString(title, stream);
        stream.writeInt(xPosition);
        stream.writeInt(yPosition);
        stream.writeInt(zPosition);
        stream.writeInt(direction);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onPaintingEntitySpawn(this);
    }

    public override int Size()
    {
        return 24;
    }
}
