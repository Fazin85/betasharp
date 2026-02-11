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

                tagMap[tag.GetKey()] = tag;
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
            tagMap[key] = value.SetKey(key);
        }

        public void SetByte(string key, sbyte value)
        {
            tagMap[key] = new NBTTagByte(value).SetKey(key);
        }

        public void SetShort(string key, short value)
        {
            tagMap[key] = new NBTTagShort(value).SetKey(key);
        }

        public void SetInteger(string key, int value)
        {
            tagMap[key] = new NBTTagInt(value).SetKey(key);
        }

        public void SetLong(string key, long value)
        {
            tagMap[key] = new NBTTagLong(value).SetKey(key);
        }

        public void SetFloat(string key, float value)
        {
            tagMap[key] = new NBTTagFloat(value).SetKey(key);
        }

        public void SetDouble(string key, double value)
        {
            tagMap[key] = new NBTTagDouble(value).SetKey(key);
        }

        public void SetString(string key, string value)
        {
            tagMap[key] = new NBTTagString(value).SetKey(key);
        }

        public void SetByteArray(string key, byte[] value)
        {
            tagMap[key] = new NBTTagByteArray(value).SetKey(key);
        }

        public void SetCompoundTag(string key, NbtTagCompound value)
        {
            tagMap[key] = value.SetKey(key);
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
            return !tagMap.TryGetValue(key, out var value) ? (sbyte) 0 : ((NBTTagByte) value).byteValue;
        }

        public short GetShort(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? (short) 0 : ((NBTTagShort) value).shortValue;
        }

        public int GetInteger(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0 : ((NBTTagInt) value).intValue;
        }

        public long GetLong(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0L : ((NBTTagLong) value).longValue;
        }

        public float GetFloat(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0.0F : ((NBTTagFloat) value).floatValue;
        }

        public double GetDouble(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0.0D : ((NBTTagDouble) value).doubleValue;
        }

        public string GetString(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? string.Empty : ((NBTTagString) value).stringValue;
        }

        public byte[] GetByteArray(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? [] : ((NBTTagByteArray) value).byteArray;
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