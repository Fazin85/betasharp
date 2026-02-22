namespace BetaSharp.Util.Maths;

public sealed class WeightedRandomSelector<T>
{
    private readonly List<T> _items = [];
    private readonly List<int> _cumulativeWeight = [0];

    public bool Empty => _items.Count == 0;

    public void Add(T item, int weight)
    {
        if (weight <= 0) throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be positive.");

        _items.Add(item);
        _cumulativeWeight.Add(_cumulativeWeight.Last() + weight);
    }

    // Note: You might want to ensure that it's not empty before calling this method, otherwise it will throw an exception.
    public T GetNext(JavaRandom random) => GetNext(random.NextInt(_cumulativeWeight.Last()));

    public T GetNext(int r)
    {
        if (Empty) throw new InvalidOperationException("No items to select from.");

        int index = _cumulativeWeight.BinarySearch(r);
        if (index < 0) index = ~index - 1; // If not found, BinarySearch returns the bitwise complement of the index of the next element that is larger than the search value.

        return _items[index];
    }

    public void Clear()
    {
        _items.Clear();
        _cumulativeWeight.Clear();
        _cumulativeWeight.Add(0);
    }
}
