using java.io;

namespace betareborn.NBT
{
    public sealed class NbtTagList : NBTBase
    {
        private List<NBTBase> _tagList = [];
        private byte _tagType;

        public override void WriteTagContents(DataOutput output)
        {
            if (_tagList.Count > 0)
            {
                _tagType = _tagList[0].GetTagType();
            }
            else
            {
                _tagType = 1;
            }

            output.writeByte(_tagType);
            output.writeInt(_tagList.Count);

            foreach (var tag in _tagList)
            {
                tag.WriteTagContents(output);
            }
        }

        public override void ReadTagContents(DataInput input)
        {
            _tagType = input.readByte();
            var length = input.readInt();
            _tagList = [];

            for (var index = 0; index < length; ++index)
            {
                var tag = CreateTagOfType(_tagType);
                tag.ReadTagContents(input);
                _tagList.Add(tag);
            }
        }

        public override byte GetTagType()
        {
            return 9;
        }

        public override string ToString()
        {
            return $"{_tagList.Count} entries of type {GetTagName(_tagType)}";
        }

        public void SetTag(NBTBase value)
        {
            _tagType = value.GetTagType();
            _tagList.Add(value);
        }

        public NBTBase TagAt(int value)
        {
            return _tagList[value];
        }

        public int TagCount()
        {
            return _tagList.Count;
        }
    }
}