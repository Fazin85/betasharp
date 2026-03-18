using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemLog : ItemBlock
{

    public ItemLog(int id) : base(id)
    {
        setMaxDamage(0);
        setHasSubtypes(true);
    }

    public override string getTextureId(int meta)
    {
        return Block.Log.getTexture("front", meta);
    }

    public override int getPlacementMetadata(int meta)
    {
        return meta;
    }
}
