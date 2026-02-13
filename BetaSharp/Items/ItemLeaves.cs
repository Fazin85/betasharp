using BetaSharp.Blocks;
using BetaSharp.Client.Colors;

namespace BetaSharp.Items;

public class ItemLeaves : ItemBlock
{

    public ItemLeaves(int id) : base(id)
    {
        setMaxDamage(0);
        setHasSubtypes(true);
    }

    public override int getPlacementMetadata(int meta)
    {
        return meta | 8;
    }

    public override int getTextureId(int meta)
    {
        return Block.LEAVES.getTexture(0, meta);
    }

    public override int getColorMultiplier(int leafType)
    {
        return (leafType & 1) == 1 ? FoliageColors.getSpruceColor() : ((leafType & 2) == 2 ? FoliageColors.getBirchColor() : FoliageColors.getDefaultColor());
    }
}