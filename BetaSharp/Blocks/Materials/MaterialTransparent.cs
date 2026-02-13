namespace BetaSharp.Blocks.Materials;

public class MaterialTransparent : Material
{

    public MaterialTransparent(MapColor var1) : base(var1)
    {
        setReplaceable();
    }

    public override bool isSolid()
    {
        return false;
    }

    public override bool blocksVision()
    {
        return false;
    }

    public override bool blocksMovement()
    {
        return false;
    }
}