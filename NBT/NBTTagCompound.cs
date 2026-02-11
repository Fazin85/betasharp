using java.io;
using java.util;

namespace betareborn.NBT
{
    public sealed class NbtTagCompound : NBTBase
    {
        private readonly Dictionary<string, NBTBase> tagMap = [];

        public override void WriteTagContents(DataOutput output)
        {
            foreach (var value in tagMap.Values)
            {
                WriteTag(value, output);
            }

            output.writeByte(0);
        }

        public override void ReadTagContents(DataInput input)
        {
            tagMap.Clear();

            while (true)
            {
                var tag = ReadTag(input);

                if (tag.GetTagType() is 0)
                {
                    return;
                }

                tagMap[tag.Key] = tag;
            }
        }

        public Collection func_28110_c()
        {
            throw new NotImplementedException();
        }

        public override byte GetTagType()
        {
            return 10;
        }

        public void SetTag(string key, NBTBase value)
        {
            value.Key = key;
            tagMap[key] = value;
        }

        public void SetByte(string key, sbyte value)
        {
            tagMap[key] = new NBTTagByte(value)
            {
                Key = Key
            };
        }

        public void SetShort(string key, short value)
        {
            tagMap[key] = new NBTTagShort(value)
            {
                Key = Key
            };
        }

        public void SetInteger(string key, int value)
        {
            tagMap[key] = new NBTTagInt(value)
            {
                Key = Key
            };
        }

        public void SetLong(string key, long value)
        {
            tagMap[key] = new NBTTagLong(value)
            {
                Key = Key
            };
        }

        public void SetFloat(string key, float value)
        {
            tagMap[key] = new NBTTagFloat(value)
            {
                Key = Key
            };
        }

        public void SetDouble(string key, double value)
        {
            tagMap[key] = new NBTTagDouble(value)
            {
                Key = Key
            };
        }

        public void SetString(string key, string value)
        {
            tagMap[key] = new NBTTagString(value)
            {
                Key = Key
            };
        }

        public void SetByteArray(string key, byte[] value)
        {
            tagMap[key] = new NBTTagByteArray(value)
            {
                Key = Key
            };
        }

        public void SetCompoundTag(string key, NbtTagCompound value)
        {
            value.Key = key;
            tagMap[key] = value;
        }

        public void SetBoolean(string key, bool value)
        {
            SetByte(key, (sbyte) (value ? 1 : 0));
        }

        public bool HasKey(string key)
        {
            return tagMap.ContainsKey(key);
        }

        public sbyte GetByte(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? (sbyte) 0 : ((NBTTagByte) value).Value;
        }

        public short GetShort(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? (short) 0 : ((NBTTagShort) value).Value;
        }

        public int GetInteger(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0 : ((NBTTagInt) value).Value;
        }

        public long GetLong(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0L : ((NBTTagLong) value).Value;
        }

        public float GetFloat(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0.0F : ((NBTTagFloat) value).Value;
        }

        public double GetDouble(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0.0D : ((NBTTagDouble) value).Value;
        }

        public string GetString(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? string.Empty : ((NBTTagString) value).Value;
        }

        public byte[] GetByteArray(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? [] : ((NBTTagByteArray) value).Value;
        }

        public NbtTagCompound GetCompoundTag(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? new NbtTagCompound() : (NbtTagCompound) value;
        }

        public NBTTagList GetTagList(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? new NBTTagList() : (NBTTagList) value;
        }

        public bool GetBoolean(string key)
        {
            return GetByte(key) != 0;
        }

        public override string ToString()
        {
            return $"{tagMap.Count} entries";
        }
    }
}