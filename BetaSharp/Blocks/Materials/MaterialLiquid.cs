namespace BetaSharp.Blocks.Materials
{
    public class MaterialLiquid : Material
    {

        public override bool IsFluid => true;
        public override bool IsSolid => false;
        public override bool BlocksMovement => false;

        public MaterialLiquid(MapColor mapColor) : base(mapColor)
        {
            SetReplaceable();
            SetDestroyPistonBehavior();
        }

    }

}