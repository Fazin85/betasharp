using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockCactus : Block
{
    public BlockCactus(int id, int textureId) : base(id, textureId, Material.Cactus) => setTickRandomly(true);

    public override void onTick(OnTickEvt evt)
    {
        if (evt.Level.BlocksReader.IsAir(evt.X, evt.Y + 1, evt.Z))
        {
            int heightBelow;
            for (heightBelow = 1; evt.Level.BlocksReader.GetBlockId(evt.X, evt.Y - heightBelow, evt.Z) == id; ++heightBelow)
            {
            }

            if (heightBelow < 3)
            {
                int growthStage = evt.Level.BlocksReader.GetMeta(evt.X, evt.Y, evt.Z);
                if (growthStage == 15)
                {
                    evt.Level.BlockWriter.SetBlock(evt.X, evt.Y + 1, evt.Z, id);
                    evt.Level.BlockWriter.SetBlockMeta(evt.X, evt.Y, evt.Z, 0);
                }
                else
                {
                    evt.Level.BlockWriter.SetBlockMeta(evt.X, evt.Y, evt.Z, growthStage + 1);
                }
            }
        }
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        float edgeInset = 1.0F / 16.0F;
        return new Box(x + edgeInset, y, z + edgeInset, x + 1 - edgeInset, y + 1 - edgeInset, z + 1 - edgeInset);
    }

    public override Box getBoundingBox(IBlockReader world, int x, int y, int z)
    {
        float edgeInset = 1.0F / 16.0F;
        return new Box(x + edgeInset, y, z + edgeInset, x + 1 - edgeInset, y + 1, z + 1 - edgeInset);
    }

    public override int getTexture(int side) => side == 1 ? textureId - 1 : side == 0 ? textureId + 1 : textureId;

    public override bool isFullCube() => false;

    public override bool isOpaque() => false;

    public override BlockRendererType getRenderType() => BlockRendererType.Cactus;

    public override bool canPlaceAt(CanPlaceAtCtx evt) => !base.canPlaceAt(evt) ? false : canGrow(evt.Level.BlocksReader, evt.X, evt.Y, evt.Z);

    public override void neighborUpdate(OnTickEvt evt)
    {
        if (!canGrow(evt))
        {
            dropStacks(new OnDropEvt(evt.Level, evt.X, evt.Y, evt.Z, evt.Level.BlocksReader.GetMeta(evt.X, evt.Y, evt.Z)));
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
        }
    }

    public override bool canGrow(OnTickEvt evt) => canGrow(evt.Level.BlocksReader, evt.X, evt.Y, evt.Z);

    private static bool canGrow(WorldBlockReader world, int x, int y, int z)
    {
        if (world.GetMaterial(x - 1, y, z).IsSolid)
        {
            return false;
        }

        if (world.GetMaterial(x + 1, y, z).IsSolid)
        {
            return false;
        }

        if (world.GetMaterial(x, y, z - 1).IsSolid)
        {
            return false;
        }

        if (world.GetMaterial(x, y, z + 1).IsSolid)
        {
            return false;
        }

        int blockBelowId = world.GetBlockId(x, y - 1, z);
        return blockBelowId == Cactus.id || blockBelowId == Sand.id;
    }

    public override void onEntityCollision(OnEntityCollisionEvt ctx) => ctx.Entity.damage(null, 1);
}
