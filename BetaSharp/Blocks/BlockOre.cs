using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockOre : Block
{

    public BlockOre(int id, int textureId) : base(id, textureId, Material.Stone)
    {
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return id == Block.CoalOre.id ? Item.Coal.id : (id == Block.DiamondOre.id ? Item.Diamond.id : (id == Block.LapisOre.id ? Item.Dye.id : id));
    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return id == Block.LapisOre.id ? 4 + random.NextInt(5) : 1;
    }

    protected override int getDroppedItemMeta(int blockMeta)
    {
        return id == Block.LapisOre.id ? 4 : 0;
    }
}