using java.io;
using java.util;

namespace betareborn.NBT
{
    public sealed class NBTTagCompound : NBTBase
    {
        private readonly Map tagMap = new HashMap();

        public override void writeTagContents(DataOutput output)
        {
            var iterator = tagMap.values().iterator();

            while (iterator.hasNext())
            {
                var tag = (NBTBase) iterator.next();
                writeTag(tag, output);
            }

            output.writeByte(0);
        }

        public override void readTagContents(DataInput var1)
        {
            tagMap.clear();

            while (true)
            {
                var tag = readTag(var1);

                if (tag.getType() == 0)
                {
                    return;
                }

                tagMap.put(tag.getKey(), tag);
            }
        }

        public Collection func_28110_c()
        {
            return tagMap.values();
        }

        public override byte getType()
        {
            return 10;
        }

        public void setTag(string key, NBTBase value)
        {
            tagMap.put(key, value.setKey(key));
        }

        public void setByte(string key, sbyte value)
        {
            tagMap.put(key, (new NBTTagByte(value)).setKey(key));
        }

        public void setShort(string key, short value)
        {
            tagMap.put(key, (new NBTTagShort(value)).setKey(key));
        }

        public void setInteger(string key, int value)
        {
            tagMap.put(key, (new NBTTagInt(value)).setKey(key));
        }

        public void setLong(string key, long value)
        {
            tagMap.put(key, (new NBTTagLong(value)).setKey(key));
        }

        public void setFloat(string key, float value)
        {
            tagMap.put(key, (new NBTTagFloat(value)).setKey(key));
        }

        public void setDouble(string key, double value)
        {
            tagMap.put(key, (new NBTTagDouble(value)).setKey(key));
        }

        public void setString(string key, string value)
        {
            tagMap.put(key, (new NBTTagString(value)).setKey(key));
        }

        public void setByteArray(string key, byte[] value)
        {
            tagMap.put(key, (new NBTTagByteArray(value)).setKey(key));
        }

        public void setCompoundTag(string key, NBTTagCompound value)
        {
            tagMap.put(key, value.setKey(key));
        }

        public void setBoolean(string key, bool value)
        {
            setByte(key, (sbyte) (value ? 1 : 0));
        }

        public bool hasKey(string key)
        {
            return tagMap.containsKey(key);
        }

        public sbyte getByte(string key)
        {
            return !tagMap.containsKey(key) ? (sbyte) 0 : ((NBTTagByte) tagMap.get(key)).byteValue;
        }

        public short getShort(string key)
        {
            return !tagMap.containsKey(key) ? (short) 0 : ((NBTTagShort) tagMap.get(key)).shortValue;
        }

        public int getInteger(string key)
        {
            return !tagMap.containsKey(key) ? 0 : ((NBTTagInt) tagMap.get(key)).intValue;
        }

        public long getLong(string key)
        {
            return !tagMap.containsKey(key) ? 0L : ((NBTTagLong) tagMap.get(key)).longValue;
        }

        public float getFloat(string key)
        {
            return !tagMap.containsKey(key) ? 0.0F : ((NBTTagFloat) tagMap.get(key)).floatValue;
        }

        public double getDouble(string key)
        {
            return !tagMap.containsKey(key) ? 0.0D : ((NBTTagDouble) tagMap.get(key)).doubleValue;
        }

        public string getString(string key)
        {
            return !tagMap.containsKey(key) ? "" : ((NBTTagString) tagMap.get(key)).stringValue;
        }

        public byte[] getByteArray(string key)
        {
            return !tagMap.containsKey(key) ? [] : ((NBTTagByteArray) tagMap.get(key)).byteArray;
        }

        public NBTTagCompound getCompoundTag(string key)
        {
            return !tagMap.containsKey(key) ? new NBTTagCompound() : (NBTTagCompound) tagMap.get(key);
        }

        public NBTTagList getTagList(string key)
        {
            return !tagMap.containsKey(key) ? new NBTTagList() : (NBTTagList) tagMap.get(key);
        }

        public bool getBoolean(string key)
        {
            return getByte(key) != 0;
        }

        public override string toString()
        {
            return "" + tagMap.size() + " entries";
        }
    }
}