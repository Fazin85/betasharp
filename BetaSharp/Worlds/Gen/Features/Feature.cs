namespace BetaSharp.Worlds.Gen.Features;

public abstract class Feature
{
    public abstract bool Generate(World world, java.util.Random random, int x, int y, int z);

    public virtual void prepare(double d0, double d1, double d2)
    {
    }
}