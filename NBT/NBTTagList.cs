using java.io;
using java.util;

namespace betareborn.NBT
{
    public class NBTTagList : NBTBase
    {
        private List tagList = new ArrayList();
        private byte tagType;

        public override void writeTagContents(DataOutput var1)
        {
            if (tagList.size() > 0)
            {
                tagType = ((NBTBase) tagList.get(0)).getType();
            }
            else
            {
                tagType = 1;
            }

            var1.writeByte(tagType);
            var1.writeInt(tagList.size());

            for (int var2 = 0; var2 < tagList.size(); ++var2)
            {
                ((NBTBase) tagList.get(var2)).writeTagContents(var1);
            }
        }

        public override void readTagContents(DataInput var1)
        {
            tagType = var1.readByte();
            int var2 = var1.readInt();
            tagList = new ArrayList();

            for (int var3 = 0; var3 < var2; ++var3)
            {
                NBTBase var4 = createTagOfType(tagType);
                var4.readTagContents(var1);

                tagList.add(var4);
            }
        }

        public override byte getType()
        {
            return 9;
        }

        public override string toString()
        {
            return "" + tagList.size() + " entries of type " + getTagName(tagType);
        }

        public void setTag(NBTBase var1)
        {
            tagType = var1.getType();
            tagList.add(var1);
        }

        public NBTBase tagAt(int var1)
        {
            return (NBTBase) tagList.get(var1);
        }

        public int tagCount()
        {
            return tagList.size();
        }
    }
}