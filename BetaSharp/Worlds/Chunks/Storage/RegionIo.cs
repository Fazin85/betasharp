using java.io;
using java.lang.@ref;
using java.util;

namespace BetaSharp.Worlds.Chunks.Storage;

public class RegionIo : java.lang.Object
{
    private static readonly Map cache = new HashMap();
    private static readonly object l = new();

    public static RegionFile func_22193_a(java.io.File var0, int var1, int var2)
    {
        lock (l)
        {
            java.io.File var3 = new(var0, "region");
            java.io.File var4 = new(var3, "r." + (var1 >> 5) + "." + (var2 >> 5) + ".mcr");
            Reference var5 = (Reference)cache.get(var4);
            RegionFile var6;
            if (var5 != null)
            {
                var6 = (RegionFile)var5.get();
                if (var6 != null)
                {
                    return var6;
                }
            }

            if (!var3.exists())
            {
                var3.mkdirs();
            }

            if (cache.size() >= 256)
            {
                flush();
            }

            var6 = new RegionFile(var4);
            cache.put(var4, new SoftReference(var6));
            return var6;
        }
    }

    public static void flush()
    {
        lock (l)
        {
            Iterator var0 = cache.values().iterator();

            while (var0.hasNext())
            {
                Reference var1 = (Reference)var0.next();

                try
                {
                    RegionFile var2 = (RegionFile)var1.get();
                    if (var2 != null)
                    {
                        var2.func_22196_b();
                    }
                }
                catch (java.io.IOException ex)
                {
                    ex.printStackTrace();
                }
            }

            cache.clear();
        }
    }

    public static int getSizeDelta(java.io.File var0, int var1, int var2)
    {
        RegionFile var3 = func_22193_a(var0, var1, var2);
        return var3.func_22209_a();
    }

    public static ChunkDataStream getChunkInputStream(java.io.File var0, int var1, int var2)
    {
        RegionFile var3 = func_22193_a(var0, var1, var2);
        return var3.getChunkDataInputStream(var1 & 31, var2 & 31);
    }

    public static Stream getChunkOutputStream(java.io.File var0, int var1, int var2)
    {
        RegionFile var3 = func_22193_a(var0, var1, var2);
        return var3.getChunkDataOutputStream(var1 & 31, var2 & 31);
    }
}