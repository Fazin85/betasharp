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
        return id == Block.CoalOre.id ? Item.Coal.id : (id == Block.DiamondOre.id ? Item.Diamond.id : (id == Block.LapisOre.id ? Item.Dye.id : id));
    }

    public override int getDroppedItemCount(java.util.Random random)
    {
        return id == Block.LapisOre.id ? 4 + random.nextInt(5) : 1;
    }

    protected override int getDroppedItemMeta(int blockMeta)
    {
        return id == Block.LapisOre.id ? 4 : 0;
    }
}