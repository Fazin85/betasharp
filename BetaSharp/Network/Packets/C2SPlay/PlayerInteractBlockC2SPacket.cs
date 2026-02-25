using BetaSharp.Items;
using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerInteractBlockC2SPacket : Packet
{
    public int x;
    public int y;
    public int z;
    public int side;
    public ItemStack stack;

    public PlayerInteractBlockC2SPacket()
    {
    }

    public PlayerInteractBlockC2SPacket(int x, int y, int z, int side, ItemStack stack)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.side = side;
        this.stack = stack;
    }

    public override void Read(DataInputStream stream)
    {
        x = stream.readInt();
        y = stream.read();
        z = stream.readInt();
        side = stream.read();
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
        stream.writeInt(x);
        stream.write(y);
        stream.writeInt(z);
        stream.write(side);
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

    public override void Apply(NetHandler handler)
    {
        handler.onPlayerInteractBlock(this);
    }

    public override int Size()
    {
        return 15;
    }
}