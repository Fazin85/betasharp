namespace BetaSharp.Util.Maths;

/// <summary>
/// C# port of Java's 48-bit LCG (Linear Congruential Generator) from java.util.Random.
/// Original source: https://github.com/openjdk/jdk/blob/master/src/java.base/share/classes/java/util/Random.java
/// Implements the exact algorithm: seed = (seed * 0x5DEECE66DL + 0xBL) &amp; ((1L &lt;&lt; 48) - 1)
/// </summary>
public class RandomFoo
{
    private const long Multiplier = 0x5DEECE66DL;
    private const long Addend = 0xBL;
    private const long Mask = (1L << 48) - 1;

    private long _seed;

    public RandomFoo(long seed)
    {
        SetSeed(seed);
    }

    public void SetSeed(long seed)
    {
        _seed = InitialScramble(seed);
    }

    private static long InitialScramble(long seed)
    {
        return (seed ^ Multiplier) & Mask;
    }

    /// <summary>
    /// Port of Java's next(bits). Returns up to 32 random bits.
    /// </summary>
    private int Next(int bits)
    {
        _seed = (_seed * Multiplier + Addend) & Mask;
        return (int)(_seed >> (48 - bits));
    }

    /// <summary>
    /// Port of Java's nextInt() - returns next(32).
    /// </summary>
    public int NextInt()
    {
        return Next(32);
    }
}