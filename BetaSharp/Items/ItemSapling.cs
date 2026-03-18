using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemSapling : ItemBlock
{

    public ItemSapling(int id) : base(id)
    {
        setMaxDamage(0);
        setHasSubtypes(true);
    }

    public override int getPlacementMetadata(int meta)
    {
        return meta;
    }

    public override string getTextureId(int meta)
    {
        return Block.Sapling.getTexture("front", meta);
    }
}
