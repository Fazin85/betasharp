using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagEnd : NBTBase
{
    public override void ReadTagContents(DataInput input)
    {
        throw new InvalidOperationException("Cannot read end tag");
    }

    public override void WriteTagContents(DataOutput output)
    {
        throw new InvalidOperationException("Cannot write end tag");
    }

    public override byte GetTagType()
    {
        return 0;
    }

    public override string ToString()
    {
        return "END";
    }
}