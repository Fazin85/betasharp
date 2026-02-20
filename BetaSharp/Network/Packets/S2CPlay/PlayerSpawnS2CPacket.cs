using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayerSpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerSpawnS2CPacket).TypeHandle);

    public int entityId;
    public string name;
    public int xPosition;
    public int yPosition;
    public int zPosition;
    public sbyte rotation;
    public sbyte pitch;
    public int currentItem;

    public PlayerSpawnS2CPacket()
    {
    }

    public PlayerSpawnS2CPacket(EntityPlayer ent)
    {
        entityId = ent.id;
        name = ent.name;
        xPosition = MathHelper.Floor(ent.x * 32.0D);
        yPosition = MathHelper.Floor(ent.y * 32.0D);
        zPosition = MathHelper.Floor(ent.z * 32.0D);
        rotation = (sbyte)(int)(ent.yaw * 256.0F / 360.0F);
        pitch = (sbyte)(int)(ent.pitch * 256.0F / 360.0F);
        ItemStack itemStack = ent.inventory.getSelectedItem();
        currentItem = itemStack == null ? 0 : itemStack.itemId;
    }

    public override void read(DataInputStream stream)
    {
        entityId = stream.readInt();
        name = readString(stream, 16);
        xPosition = stream.readInt();
        yPosition = stream.readInt();
        zPosition = stream.readInt();
        rotation = (sbyte)stream.readByte();
        pitch = (sbyte)stream.readByte();
        currentItem = stream.readShort();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        writeString(name, stream);
        stream.writeInt(xPosition);
        stream.writeInt(yPosition);
        stream.writeInt(zPosition);
        stream.writeByte(rotation);
        stream.writeByte(pitch);
        stream.writeShort(currentItem);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerSpawn(this);
    }

    public override int size()
    {
        return 28;
    }
}