using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ItemEntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ItemEntitySpawnS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public sbyte velocityX;
    public sbyte velocityY;
    public sbyte velocityZ;
    public int itemRawId;
    public int itemCount;
    public int itemDamage;

    public ItemEntitySpawnS2CPacket()
    {
    }

    public ItemEntitySpawnS2CPacket(EntityItem item)
    {
        id = item.id;
        itemRawId = item.stack.itemId;
        itemCount = item.stack.count;
        itemDamage = item.stack.getDamage();
        x = MathHelper.Floor(item.x * 32.0D);
        y = MathHelper.Floor(item.y * 32.0D);
        z = MathHelper.Floor(item.z * 32.0D);
        velocityX = (sbyte)(int)(item.velocityX * 128.0D);
        velocityY = (sbyte)(int)(item.velocityY * 128.0D);
        velocityZ = (sbyte)(int)(item.velocityZ * 128.0D);
    }

    public override void read(DataInputStream stream)
    {
        id = stream.readInt();
        itemRawId = stream.readShort();
        itemCount = (sbyte)stream.readByte();
        itemDamage = stream.readShort();
        x = stream.readInt();
        y = stream.readInt();
        z = stream.readInt();
        velocityX = (sbyte)stream.readByte();
        velocityY = (sbyte)stream.readByte();
        velocityZ = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeShort(itemRawId);
        stream.writeByte(itemCount);
        stream.writeShort(itemDamage);
        stream.writeInt(x);
        stream.writeInt(y);
        stream.writeInt(z);
        stream.writeByte(velocityX);
        stream.writeByte(velocityY);
        stream.writeByte(velocityZ);
    }

    public override void apply(NetHandler handler)
    {
        handler.onItemEntitySpawn(this);
    }

    public override int size()
    {
        return 24;
    }
}