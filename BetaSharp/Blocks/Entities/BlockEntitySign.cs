using BetaSharp.NBT;
using BetaSharp.Network.Packets;
using BetaSharp.Network.Packets.Play;
using java.lang;

namespace BetaSharp.Blocks.Entities;

public class BlockEntitySign : BlockEntity
{
    public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(BlockEntitySign).TypeHandle);
    public string[] texts = ["", "", "", ""];
    public int currentRow = -1;
    private bool editable = true;

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
        nbt.SetString("Text1", texts[0]);
        nbt.SetString("Text2", texts[1]);
        nbt.SetString("Text3", texts[2]);
        nbt.SetString("Text4", texts[3]);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        editable = false;
        base.readNbt(nbt);

        for (int line = 0; line < 4; ++line)
        {
            texts[line] = nbt.GetString("Text" + (line + 1));
            if (texts[line].Length > 15)
            {
                texts[line] = texts[line].Substring(0, 15);
            }
        }

    }

    public override Packet createUpdatePacket()
    {
        string[] lines = new string[4];

        for (int lineIndex = 0; lineIndex < 4; lineIndex++)
        {
            lines[lineIndex] = texts[lineIndex];
        }

        return new UpdateSignPacket(x, y, z, lines);
    }

    public bool isEditable()
    {
        return editable;
    }

    public void setEditable(bool editable)
    {
        this.editable = editable;
    }
}