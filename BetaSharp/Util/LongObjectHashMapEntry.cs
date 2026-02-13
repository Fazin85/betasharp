namespace BetaSharp.Util;

public class LongObjectHashMapEntry : java.lang.Object
{
    public readonly long? key;
    public object value;
    public LongObjectHashMapEntry next;
    public readonly int hash;

    public LongObjectHashMapEntry(int hash, long? key, object value, LongObjectHashMapEntry next)
    {
        this.value = value;
        this.next = next;
        this.key = key;
        this.hash = hash;
    }

    public long? getKey()
    {
        return key;
    }

    public object getValue()
    {
        return value;
    }


    public override bool equals(object obj)
    {
        if (obj is not LongObjectHashMapEntry)
        {
            return false;
        }
        else
        {
            LongObjectHashMapEntry var2 = (LongObjectHashMapEntry)obj;
            long? var3 = getKey();
            long? var4 = var2.getKey();
            if (var3 == var4 || var3 != null && var3.Equals(var4))
            {
                object var5 = getValue();
                object var6 = var2.getValue();
                if (var5 == var6 || var5 != null && var5.Equals(var6))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public override int hashCode()
    {
        return LongObjectHashMap.hash(key!.Value);
    }

    public override string toString()
    {
        return getKey() + "=" + getValue();
    }
}