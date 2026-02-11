using java.io;
using java.util;

namespace betareborn.NBT
{
    public class NBTTagCompound : NBTBase
    {
        private readonly Map tagMap = new HashMap();

        public override void writeTagContents(DataOutput var1)
        {
            Iterator var2 = tagMap.values().iterator();

            while (var2.hasNext())
            {
                NBTBase var3 = (NBTBase) var2.next();
                writeTag(var3, var1);
            }

            var1.writeByte(0);
        }

        public override void readTagContents(DataInput var1)
        {
            tagMap.clear();

            while (true)
            {
                NBTBase var2 = readTag(var1);

                if (var2.getType() == 0)
                {
                    return;
                }

                tagMap.put(var2.getKey(), var2);
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

        public void setTag(string var1, NBTBase var2)
        {
            tagMap.put(var1, var2.setKey(var1));
        }

        public void setByte(string var1, sbyte var2)
        {
            tagMap.put(var1, (new NBTTagByte(var2)).setKey(var1));
        }

        public void setShort(string var1, short var2)
        {
            tagMap.put(var1, (new NBTTagShort(var2)).setKey(var1));
        }

        public void setInteger(string var1, int var2)
        {
            tagMap.put(var1, (new NBTTagInt(var2)).setKey(var1));
        }

        public void setLong(string var1, long var2)
        {
            tagMap.put(var1, (new NBTTagLong(var2)).setKey(var1));
        }

        public void setFloat(string var1, float var2)
        {
            tagMap.put(var1, (new NBTTagFloat(var2)).setKey(var1));
        }

        public void setDouble(string var1, double var2)
        {
            tagMap.put(var1, (new NBTTagDouble(var2)).setKey(var1));
        }

        public void setString(string var1, string var2)
        {
            tagMap.put(var1, (new NBTTagString(var2)).setKey(var1));
        }

        public void setByteArray(string var1, byte[] var2)
        {
            tagMap.put(var1, (new NBTTagByteArray(var2)).setKey(var1));
        }

        public void setCompoundTag(string var1, NBTTagCompound var2)
        {
            tagMap.put(var1, var2.setKey(var1));
        }

        public void setBoolean(string var1, bool var2)
        {
            setByte(var1, (sbyte) (var2 ? 1 : 0));
        }

        public bool hasKey(string var1)
        {
            return tagMap.containsKey(var1);
        }

        public sbyte getByte(string var1)
        {
            return !tagMap.containsKey(var1) ? (sbyte) 0 : ((NBTTagByte) tagMap.get(var1)).byteValue;
        }

        public short getShort(string var1)
        {
            return !tagMap.containsKey(var1) ? (short) 0 : ((NBTTagShort) tagMap.get(var1)).shortValue;
        }

        public int getInteger(string var1)
        {
            return !tagMap.containsKey(var1) ? 0 : ((NBTTagInt) tagMap.get(var1)).intValue;
        }

        public long getLong(string var1)
        {
            return !tagMap.containsKey(var1) ? 0L : ((NBTTagLong) tagMap.get(var1)).longValue;
        }

        public float getFloat(string var1)
        {
            return !tagMap.containsKey(var1) ? 0.0F : ((NBTTagFloat) tagMap.get(var1)).floatValue;
        }

        public double getDouble(string var1)
        {
            return !tagMap.containsKey(var1) ? 0.0D : ((NBTTagDouble) tagMap.get(var1)).doubleValue;
        }

        public string getString(string var1)
        {
            return !tagMap.containsKey(var1) ? "" : ((NBTTagString) tagMap.get(var1)).stringValue;
        }

        public byte[] getByteArray(string var1)
        {
            return !tagMap.containsKey(var1) ? [] : ((NBTTagByteArray) tagMap.get(var1)).byteArray;
        }

        public NBTTagCompound getCompoundTag(string var1)
        {
            return !tagMap.containsKey(var1) ? new NBTTagCompound() : (NBTTagCompound) tagMap.get(var1);
        }

        public NBTTagList getTagList(string var1)
        {
            return !tagMap.containsKey(var1) ? new NBTTagList() : (NBTTagList) tagMap.get(var1);
        }

        public bool getBoolean(string var1)
        {
            return getByte(var1) != 0;
        }

        public override string toString()
        {
            return "" + tagMap.size() + " entries";
        }
    }
}