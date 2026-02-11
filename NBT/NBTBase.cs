using java.io;

namespace betareborn.NBT
{
    public abstract class NBTBase : java.lang.Object
    {
        private string? key;

        public abstract void writeTagContents(DataOutput output);

        public abstract void readTagContents(DataInput input);

        public abstract byte getType();

        public string getKey()
        {
            return key ?? string.Empty;
        }

        public NBTBase setKey(string value)
        {
            key = value;
            return this;
        }

        public static NBTBase readTag(DataInput input)
        {
            var identifier = input.readByte();

            if (identifier is 0)
            {
                return new NBTTagEnd();
            }

            var tag = createTagOfType(identifier);

            tag.key = input.readUTF();
            tag.readTagContents(input);

            return tag;
        }

        public static void writeTag(NBTBase tag, DataOutput output)
        {
            output.writeByte(tag.getType());

            if (tag.getType() is 0)
            {
                return;
            }

            output.writeUTF(tag.getKey());
            tag.writeTagContents(output);
        }

        public static NBTBase createTagOfType(byte identifier)
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
                10 => new NBTTagCompound(),
                _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, "Unknown NBT identifier")
            };
        }

        public static string getTagName(byte identifier)
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