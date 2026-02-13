namespace BetaSharp.Blocks.Materials;

public class MaterialPortal : Material
{

    public MaterialPortal(MapColor var1) : base(var1)
    {
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