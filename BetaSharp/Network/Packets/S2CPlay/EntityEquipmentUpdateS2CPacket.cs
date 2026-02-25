using BetaSharp.Items;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityEquipmentUpdateS2CPacket : Packet
{
    public int id;
    public int slot;
    public int itemRawId;
    public int itemDamage;

    public EntityEquipmentUpdateS2CPacket()
    {
    }

    public EntityEquipmentUpdateS2CPacket(int id, int slot, ItemStack itemStack)
    {
        this.id = id;
        this.slot = slot;
        if (itemStack == null)
        {
            itemRawId = -1;
            itemDamage = 0;
        }
        else
        {
            itemRawId = itemStack.itemId;
            itemDamage = itemStack.getDamage();
        }
    }

    public override void Read(DataInputStream stream)
    {
        id = stream.readInt();
        slot = stream.readShort();
        itemRawId = stream.readShort();
        itemDamage = stream.readShort();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeShort(slot);
        stream.writeShort(itemRawId);
        stream.writeShort(itemDamage);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityEquipmentUpdate(this);
    }

    public override int Size()
    {
        return 8;
    }
}