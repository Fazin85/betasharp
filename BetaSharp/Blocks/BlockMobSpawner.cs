using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockMobSpawner : BlockWithEntity
{

    public BlockMobSpawner(int id, int textureId) : base(id, textureId, Material.Stone)
    {
    }

    protected override BlockEntity getBlockEntity()
    {
        return new BlockEntityMobSpawner();
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return 0;
    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return 0;
    }

    public override bool isOpaque()
    {
        return false;
    }
}