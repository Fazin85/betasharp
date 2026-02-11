using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagList : NBTBase
    {
        private List<NBTBase> tagList = [];
        private byte tagType;

        public override void writeTagContents(DataOutput output)
        {
            if (tagList.Count > 0)
            {
                tagType = tagList[0].getType();
            }
            else
            {
                tagType = 1;
            }

            output.writeByte(tagType);
            output.writeInt(tagList.Count);

            foreach (var tag in tagList)
            {
                tag.writeTagContents(output);
            }
        }

        public override void readTagContents(DataInput input)
        {
            tagType = input.readByte();
            var length = input.readInt();
            tagList = [];

            for (var index = 0; index < length; ++index)
            {
                var tag = createTagOfType(tagType);
                tag.readTagContents(input);
                tagList.Add(tag);
            }
        }

        public override byte getType()
        {
            return 9;
        }

        public override string toString()
        {
            return $"{tagList.Count} entries of type {getTagName(tagType)}";
        }

        public void setTag(NBTBase value)
        {
            tagType = value.getType();
            tagList.Add(value);
        }

        public NBTBase tagAt(int value)
        {
            return tagList[value];
        }

        public int tagCount()
        {
            return tagList.Count;
        }
    }
}