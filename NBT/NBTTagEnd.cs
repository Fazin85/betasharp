using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagEnd : NBTBase
    {
        public override void readTagContents(DataInput input)
        {
            throw new InvalidOperationException("Cannot read end tag");
        }

        public override void writeTagContents(DataOutput output)
        {
            throw new InvalidOperationException("Cannot write end tag");
        }

        public override byte getType()
        {
            return 0;
        }

        public override string toString()
        {
            return "END";
        }
    }
}