using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockSoulSand : Block
{

    public BlockSoulSand(int id, int textureId) : base(id, textureId, Material.Sand)
    {
    }

    public override Box? getCollisionShape(World world, int x, int y, int z)
    {
        float height = 2.0F / 16.0F;
        return new Box((double)x, (double)y, (double)z, (double)(x + 1), (double)((float)(y + 1) - height), (double)(z + 1));
    }

    public override void onEntityCollision(World world, int x, int y, int z, Entity entity)
    {
        entity.velocityX *= 0.4D;
        entity.velocityZ *= 0.4D;
    }
}