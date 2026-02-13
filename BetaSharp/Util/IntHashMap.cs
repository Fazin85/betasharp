using java.lang;

namespace BetaSharp.Util;

public class IntHashMap : java.lang.Object
{
    [NonSerialized]
    private IntHashMapEntry[] table = new IntHashMapEntry[16];
    [NonSerialized]
    private int count;
    private int threshold = 12;
    private readonly float growFactor = 12.0F / 16.0F;

    private static int hash(int var0)
    {
        var0 ^= var0 >>> 20 ^ var0 >>> 12;
        return var0 ^ var0 >>> 7 ^ var0 >>> 4;
    }

    private static int indexOf(int var0, int var1)
    {
        return var0 & var1 - 1;
    }

    public bool containsKey(int key)
    {
        return getEntry(key) != null;
    }

    public java.lang.Object get(int var1)
    {
        int var2 = hash(var1);

        for (IntHashMapEntry var3 = table[indexOf(var2, table.Length)]; var3 != null; var3 = var3.nextEntry)
        {
            if (var3.hashEntry == var1)
            {
                return var3.valueEntry;
            }
        }

        return null;
    }

    private IntHashMapEntry getEntry(int key)
    {
        int var2 = hash(key);

        for (IntHashMapEntry var3 = table[indexOf(var2, table.Length)]; var3 != null; var3 = var3.nextEntry)
        {
            if (var3.hashEntry == key)
            {
                return var3;
            }
        }

        return null;
    }

    public void put(int var1, java.lang.Object var2)
    {
        int var3 = hash(var1);
        int var4 = indexOf(var3, table.Length);

        for (IntHashMapEntry var5 = table[var4]; var5 != null; var5 = var5.nextEntry)
        {
            if (var5.hashEntry == var1)
            {
                var5.valueEntry = var2;
            }
        }

        insert(var3, var1, var2, var4);
    }

    private void grow(int var1)
    {
        IntHashMapEntry[] var2 = table;
        int var3 = var2.Length;
        if (var3 == 1073741824)
        {
            threshold = Integer.MAX_VALUE;
        }
        else
        {
            IntHashMapEntry[] var4 = new IntHashMapEntry[var1];
            copyTo(var4);
            table = var4;
            threshold = (int)(var1 * growFactor);
        }
    }

    private void copyTo(IntHashMapEntry[] var1)
    {
        IntHashMapEntry[] var2 = table;
        int var3 = var1.Length;

        for (int var4 = 0; var4 < var2.Length; ++var4)
        {
            IntHashMapEntry var5 = var2[var4];
            if (var5 != null)
            {
                var2[var4] = null;

                IntHashMapEntry var6;
                do
                {
                    var6 = var5.nextEntry;
                    int var7 = indexOf(var5.slotHash, var3);
                    var5.nextEntry = var1[var7];
                    var1[var7] = var5;
                    var5 = var6;
                } while (var6 != null);
            }
        }

    }

    public java.lang.Object remove(int var1)
    {
        IntHashMapEntry var2 = removeEntry(var1);
        return var2 == null ? null : var2.valueEntry;
    }

    IntHashMapEntry removeEntry(int var1)
    {
        int var2 = hash(var1);
        int var3 = indexOf(var2, table.Length);
        IntHashMapEntry var4 = table[var3];

        IntHashMapEntry var5;
        IntHashMapEntry var6;
        for (var5 = var4; var5 != null; var5 = var6)
        {
            var6 = var5.nextEntry;
            if (var5.hashEntry == var1)
            {
                --count;
                if (var4 == var5)
                {
                    table[var3] = var6;
                }
                else
                {
                    var4.nextEntry = var6;
                }

                return var5;
            }

            var4 = var5;
        }

        return var5;
    }

    public void clearMap()
    {
        IntHashMapEntry[] var1 = table;

        for (int var2 = 0; var2 < var1.Length; ++var2)
        {
            var1[var2] = null;
        }

        count = 0;
    }

    private void insert(int var1, int var2, java.lang.Object var3, int var4)
    {
        IntHashMapEntry var5 = table[var4];
        table[var4] = new IntHashMapEntry(var1, var2, var3, var5);
        if (count++ >= threshold)
        {
            grow(2 * table.Length);
        }

    }

    public static int getHash(int var0)
    {
        return hash(var0);
    }
}