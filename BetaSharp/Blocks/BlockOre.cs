using BetaSharp.Blocks.Materials;
using BetaSharp.Items;

namespace BetaSharp.Blocks;

public class BlockOre : Block
{

    public BlockOre(int id, int textureId) : base(id, textureId, Material.Stone)
    {
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return id == Block.COAL_ORE.id ? Item.COAL.id : (id == Block.DIAMOND_ORE.id ? Item.DIAMOND.id : (id == Block.LAPIS_ORE.id ? Item.DYE.id : id));
    }

    public override int getDroppedItemCount(java.util.Random random)
    {
        return id == Block.LAPIS_ORE.id ? 4 + random.nextInt(5) : 1;
    }

    protected override int getDroppedItemMeta(int blockMeta)
    {
        return id == Block.LAPIS_ORE.id ? 4 : 0;
    }
}