using System.Collections;
using System.Text;

namespace Beta3D.Util;

internal class BitList
{
    private readonly BitArray _array;

    public BitList()
    {
        _array = new BitArray(16);
        Count = 0;
    }

    public BitList(int capacity)
    {
        _array = new BitArray(Math.Max(capacity, 1));
        Count = 0;
    }

    public bool this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _array[index];
        }
        set
        {
            if ((uint)index >= (uint)Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _array[index] = value;
        }
    }

    public int Count { get; private set; }

    public void Set(int index)
    {
        if (index >= Count)
        {
            EnsureCapacity(index + 1);
            Count = index + 1;
        }
        _array[index] = true;
    }

    public int Cardinality()
    {
        int count = 0;
        for (int i = 0; i < Count; i++)
            if (_array[i]) count++;
        return count;
    }

    public int NextSetBit(int fromIndex)
    {
        for (int i = fromIndex; i < Count; i++)
            if (_array[i]) return i;
        return -1;
    }

    private void EnsureCapacity(int required)
    {
        if (required <= _array.Length) return;
        int newSize = Math.Max(required, _array.Length * 2);
        _array.Length = newSize;
    }

    public override string ToString()
    {
        var sb = new StringBuilder("{");
        bool first = true;
        for (int i = 0; i < Count; i++)
        {
            if (_array[i])
            {
                if (!first) sb.Append(", ");
                sb.Append(i);
                first = false;
            }
        }
        sb.Append('}');
        return sb.ToString();
    }
}
