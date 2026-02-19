using System.Runtime.CompilerServices;

namespace BetaSharp.Util.Maths;

/// <summary>
/// C# port of Java's 48-bit LCG (Linear Congruential Generator) from Random.
/// Original source: https://github.com/openjdk/jdk/blob/master/src/java.base/share/classes/Random.java
/// Implements the exact algorithm: seed = (seed * 0x5DEECE66DL + 0xBL) &amp; ((1L &lt;&lt; 48) - 1)
/// </summary>
public class JavaRandom
{
    private static long _seedUniquifier = 8682522807148012L;
    private readonly object _lock = new object();
    private readonly object _seedLock = new object();
    private bool _haveNextNextGaussian = false;
    private double _nextNextGaussian;
    private const float FloatUnit = 1.0f / (1 << 24);
    private const double DoubleUnit = 1.0 / (1L << 53);
    private const long Multiplier = 0x5DEECE66DL;
    private const long Addend = 0xBL;
    private const long Mask = (1L << 48) - 1;

    private long _seed;

    public JavaRandom(long seed)
    {
        SetSeed(seed);
    }

    public JavaRandom() : this(SeedUniquifier() ^ DateTime.UtcNow.Ticks) { }

    public void SetSeed(long seed)
    {
        lock (_lock)
        {
            Interlocked.Exchange(ref _seed, InitialScramble(seed));
            _haveNextNextGaussian = false;
        }
    }

    private static long InitialScramble(long seed) => (seed ^ Multiplier) & Mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Next(int bits)
    {
        long nextSeed;

        lock (_seedLock)
        {
            _seed = (_seed * Multiplier + Addend) & Mask;
            nextSeed = _seed;
        }

        return (int)((ulong)nextSeed >> (48 - bits));
    }

    public int NextInt() => Next(32);

    public float NextFloat() => Next(24) * FloatUnit;

    public long NextLong() => ((long)Next(32) << 32) + Next(32);

    public double NextDouble() => (((long)Next(26) << 27) + Next(27)) * DoubleUnit;

    public bool NextBoolean() => Next(1) != 0;

    public int NextInt(int bound)
    {
        if (bound <= 0)
            throw new ArgumentException("bound must be positive");

        int r = Next(31);
        int m = bound - 1;

        if ((bound & m) == 0)
        {
            r = (int)((bound * (long)r) >> 31);
        }
        else
        {
            for (int u = r; u - (r = u % bound) + m < 0; u = Next(31)) { }
        }

        return r;
    }



    public double NextGaussian()
    {
        lock (_lock)
        {
            // See Knuth, TAOCP, Vol. 2, 3rd edition, Section 3.4.1 Algorithm C.
            if (_haveNextNextGaussian)
            {
                _haveNextNextGaussian = false;
                return _nextNextGaussian;
            }

            double v1, v2, s;
            do
            {
                v1 = 2 * NextDouble() - 1; // between -1 and 1
                v2 = 2 * NextDouble() - 1; // between -1 and 1
                s = v1 * v1 + v2 * v2;
            } while (s >= 1 || s == 0);

            // Math.Log and Math.Sqrt in C# are equivalent to Java's StrictMath
            double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);

            _nextNextGaussian = v2 * multiplier;
            _haveNextNextGaussian = true;

            return v1 * multiplier;
        }
    }

    private static long SeedUniquifier()
    {
        while (true)
        {
            long current = Interlocked.Read(ref _seedUniquifier);
            long next = current * 1181783497276652981L;

            if (Interlocked.CompareExchange(ref _seedUniquifier, next, current) == current)
            {
                return next;
            }
        }
    }
}