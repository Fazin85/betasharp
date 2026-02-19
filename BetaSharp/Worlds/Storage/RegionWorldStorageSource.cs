using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Chunks.Storage;
using java.io;
using java.util;
using File = System.IO.File;

namespace BetaSharp.Worlds.Storage;

public class RegionWorldStorageSource : WorldStorageSource
{
    protected readonly java.io.File dir;

    public RegionWorldStorageSource(java.io.File file)
    {
        if (!file.exists())
        {
            file.mkdirs();
        }
        dir = file;
    }

    public virtual string getName()
    {
        return "Scaevolus\' McRegion";
    }

    public virtual List getAll()
    {
        ArrayList var1 = new ArrayList();
        java.io.File[] var2 = dir.listFiles();
        java.io.File[] var3 = var2;
        int var4 = var2.Length;

        for (int var5 = 0; var5 < var4; ++var5)
        {
            java.io.File var6 = var3[var5];
            if (var6.isDirectory())
            {
                string var7 = var6.getName();
                WorldProperties var8 = getProperties(var7);
                if (var8 != null)
                {
                    bool var9 = var8.SaveVersion != 19132;
                    string var10 = var8.LevelName;
                    if (var10 == null || MathHelper.stringNullOrLengthZero(var10))
                    {
                        var10 = var7;
                    }

                    var1.add(new WorldSaveInfo(var7, var10, var8.LastTimePlayed, var8.SizeOnDisk, var9));
                }
            }
        }

        return var1;
    }

    public virtual void flush()
    {
        RegionIo.flush();
    }

    public virtual WorldStorage get(string var1, bool var2)
    {
        return new RegionWorldStorage(dir, var1, var2);
    }

    private static long getFolderSizeMB(java.io.File folder)
    {
        long totalSize = 0;
        java.io.File[] files = folder.listFiles();

        if (files != null)
        {
            foreach (java.io.File file in files)
            {
                if (file.isFile())
                {
                    totalSize += file.length();
                }
                else if (file.isDirectory())
                {
                    totalSize += getFolderSizeMB(file);
                }
            }
        }

        return totalSize;
    }

    public virtual WorldProperties getProperties(string var1)
    {
        java.io.File var2 = new java.io.File(dir, var1);
        if (!var2.exists())
        {
            return null;
        }
        else
        {
            java.io.File file = new java.io.File(var2, "level.dat");
            NBTTagCompound var4;
            NBTTagCompound var5;
            if (file.exists())
            {
                try
                {
                    using var stream = File.OpenRead(file.getAbsolutePath());
                    var4 = NbtIo.ReadCompressed(stream);

                    var5 = var4.GetCompoundTag("Data");
                    long sizeOnDisk = getFolderSizeMB(var2);
                    var wInfo = new WorldProperties(var5);
                    wInfo.SizeOnDisk = sizeOnDisk;
                    return wInfo;
                }
                catch (java.lang.Exception ex)
                {
                    ex.printStackTrace();
                }
            }

            file = new java.io.File(var2, "level.dat_old");
            if (file.exists())
            {
                try
                {
                    using var stream = File.OpenRead(file.getAbsolutePath());
                    var4 = NbtIo.ReadCompressed(stream);

                    var5 = var4.GetCompoundTag("Data");
                    long sizeOnDisk = getFolderSizeMB(var2);
                    var wInfo = new WorldProperties(var5);
                    wInfo.SizeOnDisk = sizeOnDisk;
                    return wInfo;
                }
                catch (java.lang.Exception ex)
                {
                    ex.printStackTrace();
                }
            }

            return null;
        }
    }

    public void rename(string var1, string var2)
    {
        java.io.File var3 = new java.io.File(dir, var1);
        if (var3.exists())
        {
            java.io.File file = new java.io.File(var3, "level.dat");
            if (file.exists())
            {
                try
                {
                    using var readingStream = File.OpenRead(file.getAbsolutePath());
                    NBTTagCompound tag = NbtIo.ReadCompressed(readingStream);

                    NBTTagCompound var6 = tag.GetCompoundTag("Data");
                    var6.SetString("LevelName", var2);

                    using var writingStream = File.OpenWrite(file.getAbsolutePath());
                    NbtIo.WriteCompressed(tag, writingStream);
                }
                catch (java.lang.Exception ex)
                {
                    ex.printStackTrace();
                }
            }

        }
    }

    public void delete(string var1)
    {
        java.io.File var2 = new java.io.File(dir, var1);
        if (var2.exists())
        {
            func_22179_a(var2.listFiles());
            var2.delete();
        }
    }

    protected static void func_22179_a(java.io.File[] var0)
    {
        for (int var1 = 0; var1 < var0.Length; ++var1)
        {
            if (var0[var1].isDirectory())
            {
                func_22179_a(var0[var1].listFiles());
            }

            var0[var1].delete();
        }

    }
}