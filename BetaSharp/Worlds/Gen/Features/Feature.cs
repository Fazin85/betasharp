using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Gen.Features;

public abstract class Feature
{
    public abstract bool Generate(World world, JavaRandom random, int x, int y, int z);

    public virtual void prepare(double d0, double d1, double d2)
    {
    }
}