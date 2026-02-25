using BetaSharp.Items;
using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class ClickSlotC2SPacket : Packet
{
    public int syncId;
    public int slot;
    public int button;
    public short actionType;
    public ItemStack stack;
    public bool holdingShift;

    public ClickSlotC2SPacket()
    {
    }

    public ClickSlotC2SPacket(int syncId, int slot, int button, bool holdingShift, ItemStack stack, short actionType)
    {
        this.syncId = syncId;
        this.slot = slot;
        this.button = button;
        this.stack = stack;
        this.actionType = actionType;
        this.holdingShift = holdingShift;
    }

    public override void Apply(NetHandler handler)
    {
        handler.onClickSlot(this);
    }

    public override void Read(DataInputStream stream)
    {
        syncId = (sbyte)stream.readByte();
        slot = stream.readShort();
        button = (sbyte)stream.readByte();
        actionType = stream.readShort();
        holdingShift = stream.readBoolean();
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
        stream.writeByte(button);
        stream.writeShort(actionType);
        stream.writeBoolean(holdingShift);
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
        return 11;
    }
}