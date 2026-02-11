using java.io;
using java.util;

namespace betareborn.NBT
{
    public sealed class NBTTagCompound : NBTBase
    {
        private readonly Dictionary<string, NBTBase> tagMap = [];

        public override void writeTagContents(DataOutput output)
        {
            foreach (var value in tagMap.Values)
            {
                writeTag(value, output);
            }

            output.writeByte(0);
        }

        public override void readTagContents(DataInput input)
        {
            tagMap.Clear();

            while (true)
            {
                var tag = readTag(input);

                if (tag.getType() is 0)
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

        public override byte getType()
        {
            return 10;
        }

        public void setTag(string key, NBTBase value)
        {
            value.Key = key;
            tagMap[key] = value;
        }

        public void setByte(string key, sbyte value)
        {
            tagMap[key] = new NBTTagByte(value)
            {
                Key = Key
            };
        }

        public void setShort(string key, short value)
        {
            tagMap[key] = new NBTTagShort(value)
            {
                Key = Key
            };
        }

        public void setInteger(string key, int value)
        {
            tagMap[key] = new NBTTagInt(value)
            {
                Key = Key
            };
        }

        public void setLong(string key, long value)
        {
            tagMap[key] = new NBTTagLong(value)
            {
                Key = Key
            };
        }

        public void setFloat(string key, float value)
        {
            tagMap[key] = new NBTTagFloat(value)
            {
                Key = Key
            };
        }

        public void setDouble(string key, double value)
        {
            tagMap[key] = new NBTTagDouble(value)
            {
                Key = Key
            };
        }

        public void setString(string key, string value)
        {
            tagMap[key] = new NBTTagString(value)
            {
                Key = Key
            };
        }

        public void setByteArray(string key, byte[] value)
        {
            tagMap[key] = new NBTTagByteArray(value)
            {
                Key = Key
            };
        }

        public void setCompoundTag(string key, NBTTagCompound value)
        {
            value.Key = key;
            tagMap[key] = value;
        }

        public void setBoolean(string key, bool value)
        {
            setByte(key, (sbyte) (value ? 1 : 0));
        }

        public bool hasKey(string key)
        {
            return tagMap.ContainsKey(key);
        }

        public sbyte getByte(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? (sbyte) 0 : ((NBTTagByte) value).byteValue;
        }

        public short getShort(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? (short) 0 : ((NBTTagShort) value).shortValue;
        }

        public int getInteger(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0 : ((NBTTagInt) value).intValue;
        }

        public long getLong(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0L : ((NBTTagLong) value).longValue;
        }

        public float getFloat(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0.0F : ((NBTTagFloat) value).floatValue;
        }

        public double getDouble(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? 0.0D : ((NBTTagDouble) value).doubleValue;
        }

        public string getString(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? string.Empty : ((NBTTagString) value).stringValue;
        }

        public byte[] getByteArray(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? [] : ((NBTTagByteArray) value).byteArray;
        }

        public NBTTagCompound getCompoundTag(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? new NBTTagCompound() : (NBTTagCompound) value;
        }

        public NBTTagList getTagList(string key)
        {
            return !tagMap.TryGetValue(key, out var value) ? new NBTTagList() : (NBTTagList) value;
        }

        public bool getBoolean(string key)
        {
            return getByte(key) != 0;
        }

        public override string toString()
        {
            return $"{tagMap.Count} entries";
        }
    }
}