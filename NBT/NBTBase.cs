using java.io;

namespace betareborn.NBT
{
    public abstract class NBTBase
    {
        private string? key;

        public abstract void WriteTagContents(DataOutput output);

        public abstract void ReadTagContents(DataInput input);

        public abstract byte GetTagType();

        public string GetKey()
        {
            return key ?? string.Empty;
        }

        public NBTBase SetKey(string value)
        {
            key = value;
            return this;
        }

        public static NBTBase ReadTag(DataInput input)
        {
            var identifier = input.readByte();

            if (identifier is 0)
            {
                return new NBTTagEnd();
            }

            var tag = CreateTagOfType(identifier);

            tag.key = input.readUTF();
            tag.ReadTagContents(input);

            return tag;
        }

        public static void WriteTag(NBTBase tag, DataOutput output)
        {
            output.writeByte(tag.GetTagType());

            if (tag.GetTagType() is 0)
            {
                return;
            }

            output.writeUTF(tag.GetKey());
            tag.WriteTagContents(output);
        }

        public static NBTBase CreateTagOfType(byte identifier)
        {
            return identifier switch
            {
                0 => new NBTTagEnd(),
                1 => new NBTTagByte(),
                2 => new NBTTagShort(),
                3 => new NBTTagInt(),
                4 => new NBTTagLong(),
                5 => new NBTTagFloat(),
                6 => new NBTTagDouble(),
                7 => new NBTTagByteArray(),
                8 => new NBTTagString(),
                9 => new NBTTagList(),
                10 => new NbtTagCompound(),
                _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, "Unknown NBT identifier")
            };
        }

        public static string GetTagName(byte identifier)
        {
            return identifier switch
            {
                0 => "TAG_End",
                1 => "TAG_Byte",
                2 => "TAG_Short",
                3 => "TAG_Int",
                4 => "TAG_Long",
                5 => "TAG_Float",
                6 => "TAG_Double",
                7 => "TAG_Byte_Array",
                8 => "TAG_String",
                9 => "TAG_List",
                10 => "TAG_Compound",
                _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, "Unknown NBT identifier")
            };
        }
    }
}