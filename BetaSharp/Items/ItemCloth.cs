using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemCloth : ItemBlock
{

    public ItemCloth(int id) : base(id)
    {
        setMaxDamage(0);
        setHasSubtypes(true);
    }

    public override string getTextureId(int meta)
    {
        return Block.Wool.getTexture("front", BlockCloth.getBlockMeta(meta));
    }

    public override int getPlacementMetadata(int meta)
    {
        return meta;
    }

    public override String getItemNameIS(ItemStack itemStack)
    {
        return base.getItemName() + "." + ItemDye.DyeColorNames[BlockCloth.getBlockMeta(itemStack.getDamage())];
    }
}
