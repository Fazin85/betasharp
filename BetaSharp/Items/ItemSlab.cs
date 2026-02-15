using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemSlab : ItemBlock
{

    public ItemSlab(int id) : base(id)
    {
        setMaxDamage(0);
        setHasSubtypes(true);
    }

    public override int getTextureId(int meta)
    {
        return Block.Slab.getTexture(2, meta);
    }

    public override int getPlacementMetadata(int meta)
    {
        return meta;
    }

    public override String getItemNameIS(ItemStack itemStack)
    {
        return base.getItemName() + "." + BlockSlab.names[itemStack.getDamage()];
    }
}