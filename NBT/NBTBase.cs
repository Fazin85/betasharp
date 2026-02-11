using java.io;

namespace betareborn.NBT
{
    public abstract class NBTBase : java.lang.Object
    {
        private string key = null;

        public abstract void writeTagContents(DataOutput var1);

        public abstract void readTagContents(DataInput var1);

        public abstract byte getType();

        public string getKey()
        {
            return key == null ? "" : key;
        }

        public NBTBase setKey(string var1)
        {
            key = var1;
            return this;
        }

        public static NBTBase readTag(DataInput var0)
        {
            byte var1 = var0.readByte();
            if (var1 == 0)
            {
                return new NBTTagEnd();
            }
            else
            {
                NBTBase var2 = createTagOfType(var1);
                var2.key = var0.readUTF();
                var2.readTagContents(var0);
                return var2;
            }
        }

        public static void writeTag(NBTBase var0, DataOutput var1)
        {
            var1.writeByte(var0.getType());
            if (var0.getType() != 0)
            {
                var1.writeUTF(var0.getKey());
                var0.writeTagContents(var1);
            }
        }

        public static NBTBase createTagOfType(byte var0)
        {
            return var0 switch
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
                _ => null,
            };
        }

        public static string getTagName(byte var0)
        {
            return var0 switch
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
                _ => "UNKNOWN",
            };
        }
    }
}