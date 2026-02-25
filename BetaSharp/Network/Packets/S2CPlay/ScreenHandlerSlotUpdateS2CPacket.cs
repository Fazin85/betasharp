using BetaSharp.Items;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ScreenHandlerSlotUpdateS2CPacket : Packet
{
    public int syncId;
    public int slot;
    public ItemStack stack;

    public ScreenHandlerSlotUpdateS2CPacket()
    {
    }

    public ScreenHandlerSlotUpdateS2CPacket(int syncId, int slot, ItemStack stack)
    {
        this.syncId = syncId;
        this.slot = slot;
        this.stack = stack == null ? stack : stack.copy();
    }

    public override void Apply(NetHandler handler)
    {
        handler.onScreenHandlerSlotUpdate(this);
    }

    public override void Read(DataInputStream stream)
    {
        syncId = (sbyte)stream.readByte();
        slot = stream.readShort();
        short itemId = stream.readShort();
        if (itemId >= 0)
        {
            sbyte count = (sbyte)stream.readByte();
            short damage = stream.readShort();
            stack = new ItemStack(itemId, count, damage);
        }
        else
        {
            stack = null;
        }

    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeByte(syncId);
        stream.writeShort(slot);
        if (stack == null)
        {
            stream.writeShort(-1);
        }
        else
        {
            stream.writeShort(stack.itemId);
            stream.writeByte(stack.count);
            stream.writeShort(stack.getDamage());
        }

    }

    public override int Size()
    {
        return 8;
    }
}