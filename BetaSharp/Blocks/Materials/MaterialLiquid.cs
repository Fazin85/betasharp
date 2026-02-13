namespace BetaSharp.Blocks.Materials;

public class MaterialLiquid : Material
{

    public MaterialLiquid(MapColor var1) : base(var1)
    {
        setReplaceable();
        setDestroyPistonBehavior();
    }

    public override bool isFluid()
    {
        return true;
    }

    public override bool blocksMovement()
    {
        return false;
    }

    public override bool isSolid()
    {
        return false;
    }
}