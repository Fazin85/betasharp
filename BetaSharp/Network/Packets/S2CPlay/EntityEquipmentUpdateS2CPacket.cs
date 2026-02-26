using System.Net.Sockets;
using BetaSharp.Items;

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

    public override void Read(NetworkStream stream)
    {
        id = stream.ReadInt();
        slot = stream.ReadShort();
        itemRawId = stream.ReadShort();
        itemDamage = stream.ReadShort();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(id);
        stream.WriteShort((short)slot);
        stream.WriteShort((short)itemRawId);
        stream.WriteShort((short)itemDamage);
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
