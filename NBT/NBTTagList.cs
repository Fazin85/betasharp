using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagList : NBTBase
    {
        private List<NBTBase> tagList = [];
        private byte tagType;

        public override void WriteTagContents(DataOutput output)
        {
            tagType = tagList.Count > 0 ? tagList[0].GetTagType() : (byte) 1;

            output.writeByte(tagType);
            output.writeInt(tagList.Count);

            foreach (var tag in tagList)
            {
                tag.WriteTagContents(output);
            }
        }

        public override void ReadTagContents(DataInput input)
        {
            tagType = input.readByte();
            var length = input.readInt();
            tagList = [];

            for (var index = 0; index < length; ++index)
            {
                var tag = CreateTagOfType(tagType);
                tag.ReadTagContents(input);
                tagList.Add(tag);
            }
        }

        public override byte GetTagType()
        {
            return 9;
        }

        public override string ToString()
        {
            return $"{tagList.Count} entries of type {GetTagName(tagType)}";
        }

        public void setTag(NBTBase value)
        {
            tagType = value.GetTagType();
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