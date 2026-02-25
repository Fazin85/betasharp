using BetaSharp.Items;
using java.io;
using java.util;

namespace BetaSharp.Network.Packets.S2CPlay;

public class InventoryS2CPacket : Packet
{
    public int syncId;
    public ItemStack[] contents;

    public InventoryS2CPacket()
    {
    }

    public InventoryS2CPacket(int syncId, List contents)
    {
        this.syncId = syncId;
        this.contents = new ItemStack[contents.size()];

        for (int i = 0; i < this.contents.Length; i++)
        {
            ItemStack itemStack = (ItemStack)contents.get(i);
            this.contents[i] = itemStack == null ? null : itemStack.copy();
        }
    }

    public override void Read(DataInputStream stream)
    {
        syncId = (sbyte)stream.readByte();
        short itemsCount = stream.readShort();
        contents = new ItemStack[itemsCount];

        for (int i = 0; i < itemsCount; ++i)
        {
            short itemId = stream.readShort();
            if (itemId >= 0)
            {
                sbyte count = (sbyte)stream.readByte();
                short damage = stream.readShort();

                contents[i] = new ItemStack(itemId, count, damage);
            }
        }

    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeByte(syncId);
        stream.writeShort(contents.Length);

        for (int i = 0; i < contents.Length; ++i)
        {
            if (contents[i] == null)
            {
                stream.writeShort(-1);
            }
            else
            {
                stream.writeShort((short)contents[i].itemId);
                stream.writeByte((byte)contents[i].count);
                stream.writeShort((short)contents[i].getDamage());
            }
        }

    }

    public override void Apply(NetHandler handler)
    {
        handler.onInventory(this);
    }

    public override int Size()
    {
        return 3 + contents.Length * 5;
    }
}