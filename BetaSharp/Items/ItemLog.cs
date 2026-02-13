using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemLog : ItemBlock
{

    public ItemLog(int id) : base(id)
    {
        setMaxDamage(0);
        setHasSubtypes(true);
    }

    public override int getTextureId(int meta)
    {
        return Block.LOG.getTexture(2, meta);
    }

    public override int getPlacementMetadata(int meta)
    {
        return meta;
    }
}