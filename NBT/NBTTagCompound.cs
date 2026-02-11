using java.io;

namespace betareborn.NBT
{
    public sealed class NbtTagCompound : NBTBase
    {
        public IEnumerable<NBTBase> Values => _tagMap.Values;
        
        private readonly Dictionary<string, NBTBase> _tagMap = [];

        public override void WriteTagContents(DataOutput output)
        {
            foreach (var value in _tagMap.Values)
            {
                WriteTag(value, output);
            }

            output.writeByte(0);
        }

        public override void ReadTagContents(DataInput input)
        {
            _tagMap.Clear();

            while (true)
            {
                var tag = ReadTag(input);

                if (tag.GetTagType() is 0)
                {
                    return;
                }

                _tagMap[tag.Key] = tag;
            }
        }

        public override byte GetTagType()
        {
            return 10;
        }

        public void SetTag(string key, NBTBase value)
        {
            value.Key = key;
            _tagMap[key] = value;
        }

        public void SetByte(string key, sbyte value)
        {
            _tagMap[key] = new NBTTagByte(value)
            {
                Key = Key
            };
        }

        public void SetShort(string key, short value)
        {
            _tagMap[key] = new NBTTagShort(value)
            {
                Key = Key
            };
        }

        public void SetInteger(string key, int value)
        {
            _tagMap[key] = new NBTTagInt(value)
            {
                Key = Key
            };
        }

        public void SetLong(string key, long value)
        {
            _tagMap[key] = new NBTTagLong(value)
            {
                Key = Key
            };
        }

        public void SetFloat(string key, float value)
        {
            _tagMap[key] = new NBTTagFloat(value)
            {
                Key = Key
            };
        }

        public void SetDouble(string key, double value)
        {
            _tagMap[key] = new NBTTagDouble(value)
            {
                Key = Key
            };
        }

        public void SetString(string key, string value)
        {
            _tagMap[key] = new NBTTagString(value)
            {
                Key = Key
            };
        }

        public void SetByteArray(string key, byte[] value)
        {
            _tagMap[key] = new NBTTagByteArray(value)
            {
                Key = Key
            };
        }

        public void SetCompoundTag(string key, NbtTagCompound value)
        {
            value.Key = key;
            _tagMap[key] = value;
        }

        public void SetBoolean(string key, bool value)
        {
            SetByte(key, (sbyte) (value ? 1 : 0));
        }

        public bool HasKey(string key)
        {
            return _tagMap.ContainsKey(key);
        }

        public sbyte GetByte(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? (sbyte) 0 : ((NBTTagByte) value).Value;
        }

        public short GetShort(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? (short) 0 : ((NBTTagShort) value).Value;
        }

        public int GetInteger(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? 0 : ((NBTTagInt) value).Value;
        }

        public long GetLong(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? 0L : ((NBTTagLong) value).Value;
        }

        public float GetFloat(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? 0.0F : ((NBTTagFloat) value).Value;
        }

        public double GetDouble(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? 0.0D : ((NBTTagDouble) value).Value;
        }

        public string GetString(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? string.Empty : ((NBTTagString) value).Value;
        }

        public byte[] GetByteArray(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? [] : ((NBTTagByteArray) value).Values;
        }

        public NbtTagCompound GetCompoundTag(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? new NbtTagCompound() : (NbtTagCompound) value;
        }

        public NbtTagList GetTagList(string key)
        {
            return !_tagMap.TryGetValue(key, out var value) ? new NbtTagList() : (NbtTagList) value;
        }

        public bool GetBoolean(string key)
        {
            return GetByte(key) != 0;
        }

        public override string ToString()
        {
            return $"{_tagMap.Count} entries";
        }
    }
}