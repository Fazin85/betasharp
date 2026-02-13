using java.lang;

namespace BetaSharp.Util;

public class LongObjectHashMap
{
    [NonSerialized]
    private LongObjectHashMapEntry[] entries;
    [NonSerialized]
    private int size;
    private int capacity;
    private readonly float loadFactor = 0.75F;

    public LongObjectHashMap()
    {
        capacity = 12;
        entries = new LongObjectHashMapEntry[16];
    }

    public static int hash(long key)
    {
        return hash((int)(key ^ key >>> 32));
    }

    private static int hash(int key)
    {
        key ^= key >>> 20 ^ key >>> 12;
        return key ^ key >>> 7 ^ key >>> 4;
    }

    private static int indexOf(int hash, int arrayLength)
    {
        return hash & arrayLength - 1;
    }

    public object get(long key)
    {
        int var3 = hash(key);

        for (LongObjectHashMapEntry var4 = entries[indexOf(var3, entries.Length)]; var4 != null; var4 = var4.next)
        {
            if (var4.key.HasValue && var4.key.Value == key)
            {
                return var4.value;
            }
        }

        return null;
    }

    public void put(long key, object value)
    {
        int var4 = hash(key);
        int var5 = indexOf(var4, entries.Length);

        for (LongObjectHashMapEntry var6 = entries[var5]; var6 != null; var6 = var6.next)
        {
            if (var6.key.HasValue && var6.key.Value == key)
            {
                var6.value = value;
            }
        }

        addEntry(var4, key, value, var5);
    }

    private void resize(int size)
    {
        LongObjectHashMapEntry[] var2 = entries;
        int var3 = var2.Length;
        if (var3 == 1073741824)
        {
            capacity = Integer.MAX_VALUE;
        }
        else
        {
            LongObjectHashMapEntry[] var4 = new LongObjectHashMapEntry[size];
            transfer(var4);
            entries = var4;
            capacity = (int)(size * loadFactor);
        }
    }

    private void transfer(LongObjectHashMapEntry[] entryArray)
    {
        LongObjectHashMapEntry[] var2 = entries;
        int var3 = entryArray.Length;

        for (int var4 = 0; var4 < var2.Length; var4++)
        {
            LongObjectHashMapEntry var5 = var2[var4];
            if (var5 != null)
            {
                var2[var4] = null;

                while (true)
                {
                    LongObjectHashMapEntry var6 = var5.next;
                    int var7 = indexOf(var5.hash, var3);
                    var5.next = entryArray[var7];
                    entryArray[var7] = var5;
                    var5 = var6;
                    if (var6 == null)
                    {
                        break;
                    }
                }
            }
        }
    }

    public object remove(long key)
    {
        LongObjectHashMapEntry var3 = removeEntry(key);
        return var3 == null ? null : var3.value;
    }

    public LongObjectHashMapEntry removeEntry(long key)
    {
        int var3 = hash(key);
        int var4 = indexOf(var3, entries.Length);
        LongObjectHashMapEntry var5 = entries[var4];
        LongObjectHashMapEntry var6 = var5;

        while (var6 != null)
        {
            LongObjectHashMapEntry var7 = var6.next;
            if (var6.key.HasValue && var6.key.Value == key)
            {
                size--;
                if (var5 == var6)
                {
                    entries[var4] = var7;
                }
                else
                {
                    var5.next = var7;
                }

                return var6;
            }

            var5 = var6;
            var6 = var7;
        }

        return var6;
    }

    private void addEntry(int hash, long key, object value, int index)
    {
        LongObjectHashMapEntry var6 = entries[index];
        entries[index] = new LongObjectHashMapEntry(hash, key, value, var6);
        if (size++ >= capacity)
        {
            resize(2 * entries.Length);
        }
    }
}