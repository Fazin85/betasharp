using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockZombiebrineSpawner : BlockStone
{
    public BlockZombiebrineSpawner(int id, int textureId) : base(id, textureId)
    {}

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return 0;
    }

    public override void onBreak(World world, int x, int y, int z)
    {
        if (!world.isRemote)
        {
            EntityZombieBrine ent = new EntityZombieBrine(world);
            ent.setPosition(x,y,z);
            world.SpawnEntity(ent);
        }

        base.onBreak(world, x, y, z);
    }
}