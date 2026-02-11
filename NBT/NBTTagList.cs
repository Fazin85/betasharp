using java.io;
using java.util;

namespace betareborn.NBT
{
    public sealed class NBTTagList : NBTBase
    {
        private List tagList = new ArrayList();
        private byte tagType;

        public override void writeTagContents(DataOutput output)
        {
            if (tagList.size() > 0)
            {
                tagType = ((NBTBase) tagList.get(0)).getType();
            }
            else
            {
                tagType = 1;
            }

            output.writeByte(tagType);
            output.writeInt(tagList.size());

            for (var index = 0; index < tagList.size(); ++index)
            {
                ((NBTBase) tagList.get(index)).writeTagContents(output);
            }
        }

        public override void readTagContents(DataInput input)
        {
            tagType = input.readByte();
            var length = input.readInt();
            tagList = new ArrayList();

            for (var index = 0; index < length; ++index)
            {
                var tag = createTagOfType(tagType);
                tag.readTagContents(input);
                tagList.add(tag);
            }
        }

        public override byte getType()
        {
            return 9;
        }

        public override string toString()
        {
            return $"{tagList.size()} entries of type {getTagName(tagType)}";
        }

        public void setTag(NBTBase value)
        {
            tagType = value.getType();
            tagList.add(value);
        }

        public NBTBase tagAt(int value)
        {
            return (NBTBase) tagList.get(value);
        }

        public int tagCount()
        {
            return tagList.size();
        }
    }
}