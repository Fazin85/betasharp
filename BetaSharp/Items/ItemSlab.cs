using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemSlab : ItemBlock
{

    public ItemSlab(int id) : base(id)
    {
        setMaxDamage(0);
        setHasSubtypes(true);
    }

    public override string getTextureId(int meta)
    {
        return Block.Slab.getTexture("front", meta);
    }

    public override int getPlacementMetadata(int meta)
    {
        return meta;
    }

    public override String getItemNameIS(ItemStack itemStack)
    {
        if(BlockSlab.names.Length > itemStack.getDamage())
            return base.getItemName() + "." + BlockSlab.names[itemStack.getDamage()];

        return "";
    }
}
