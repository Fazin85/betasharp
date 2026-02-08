using betareborn.Entities;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Worlds;
using java.util;

namespace betareborn
{
    public class MapData : PersistentState
    {
        public static readonly java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(MapData).TypeHandle);
        public int field_28180_b;
        public int field_28179_c;
        public sbyte field_28178_d;
        public sbyte field_28177_e;
        public byte[] field_28176_f = new byte[16384];
        public int field_28175_g;
        public List field_28174_h = new ArrayList();
        private Map field_28172_j = new HashMap();
        public List field_28173_i = new ArrayList();

        public MapData(string var1) : base(new(var1))
        {
        }

        public override void readFromNBT(NBTTagCompound var1)
        {
            field_28178_d = var1.getByte("dimension");
            field_28180_b = var1.getInteger("xCenter");
            field_28179_c = var1.getInteger("zCenter");
            field_28177_e = var1.getByte("scale");
            if (field_28177_e < 0)
            {
                field_28177_e = 0;
            }

            if (field_28177_e > 4)
            {
                field_28177_e = 4;
            }

            short var2 = var1.getShort("width");
            short var3 = var1.getShort("height");
            if (var2 == 128 && var3 == 128)
            {
                field_28176_f = var1.getByteArray("colors");
            }
            else
            {
                byte[] var4 = var1.getByteArray("colors");
                field_28176_f = new byte[16384];
                int var5 = (128 - var2) / 2;
                int var6 = (128 - var3) / 2;

                for (int var7 = 0; var7 < var3; ++var7)
                {
                    int var8 = var7 + var6;
                    if (var8 >= 0 || var8 < 128)
                    {
                        for (int var9 = 0; var9 < var2; ++var9)
                        {
                            int var10 = var9 + var5;
                            if (var10 >= 0 || var10 < 128)
                            {
                                field_28176_f[var10 + var8 * 128] = var4[var9 + var7 * var2];
                            }
                        }
                    }
                }
            }

        }

        public override void writeToNBT(NBTTagCompound var1)
        {
            var1.setByte("dimension", field_28178_d);
            var1.setInteger("xCenter", field_28180_b);
            var1.setInteger("zCenter", field_28179_c);
            var1.setByte("scale", field_28177_e);
            var1.setShort("width", (short)128);
            var1.setShort("height", (short)128);
            var1.setByteArray("colors", field_28176_f);
        }

        public void func_28169_a(EntityPlayer var1, ItemStack var2)
        {
            if (!field_28172_j.containsKey(var1))
            {
                MapInfo var3 = new MapInfo(this, var1);
                field_28172_j.put(var1, var3);
                field_28174_h.add(var3);
            }

            field_28173_i.clear();

            for (int var14 = 0; var14 < field_28174_h.size(); ++var14)
            {
                MapInfo var4 = (MapInfo)field_28174_h.get(var14);
                if (!var4.entityplayerObj.isDead && var4.entityplayerObj.inventory.func_28018_c(var2))
                {
                    float var5 = (float)(var4.entityplayerObj.posX - (double)field_28180_b) / (float)(1 << field_28177_e);
                    float var6 = (float)(var4.entityplayerObj.posZ - (double)field_28179_c) / (float)(1 << field_28177_e);
                    byte var7 = 64;
                    byte var8 = 64;
                    if (var5 >= (float)(-var7) && var6 >= (float)(-var8) && var5 <= (float)var7 && var6 <= (float)var8)
                    {
                        byte var9 = 0;
                        byte var10 = (byte)((int)((double)(var5 * 2.0F) + 0.5D));
                        byte var11 = (byte)((int)((double)(var6 * 2.0F) + 0.5D));
                        byte var12 = (byte)((int)((double)(var1.rotationYaw * 16.0F / 360.0F) + 0.5D));
                        if (field_28178_d < 0)
                        {
                            int var13 = field_28175_g / 10;
                            var12 = (byte)(var13 * var13 * 34187121 + var13 * 121 >> 15 & 15);
                        }

                        if (var4.entityplayerObj.dimension == field_28178_d)
                        {
                            field_28173_i.add(new MapCoord(this, var9, var10, var11, var12));
                        }
                    }
                }
                else
                {
                    field_28172_j.remove(var4.entityplayerObj);
                    field_28174_h.remove(var4);
                }
            }

        }

        public void func_28170_a(int var1, int var2, int var3)
        {
            base.markDirty();

            for (int var4 = 0; var4 < field_28174_h.size(); ++var4)
            {
                MapInfo var5 = (MapInfo)field_28174_h.get(var4);
                if (var5.field_28119_b[var1] < 0 || var5.field_28119_b[var1] > var2)
                {
                    var5.field_28119_b[var1] = var2;
                }

                if (var5.field_28124_c[var1] < 0 || var5.field_28124_c[var1] < var3)
                {
                    var5.field_28124_c[var1] = var3;
                }
            }

        }

        public void func_28171_a(byte[] var1)
        {
            int var2;
            if (var1[0] == 0)
            {
                var2 = var1[1] & 255;
                int var3 = var1[2] & 255;

                for (int var4 = 0; var4 < var1.Length - 3; ++var4)
                {
                    field_28176_f[(var4 + var3) * 128 + var2] = var1[var4 + 3];
                }

                markDirty();
            }
            else if (var1[0] == 1)
            {
                field_28173_i.clear();

                for (var2 = 0; var2 < (var1.Length - 1) / 3; ++var2)
                {
                    byte var7 = (byte)(var1[var2 * 3 + 1] % 16);
                    byte var8 = var1[var2 * 3 + 2];
                    byte var5 = var1[var2 * 3 + 3];
                    byte var6 = (byte)(var1[var2 * 3 + 1] / 16);
                    field_28173_i.add(new MapCoord(this, var7, var8, var5, var6));
                }
            }

        }
    }

}