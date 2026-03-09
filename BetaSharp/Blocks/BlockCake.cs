using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockCake : Block
{
    public BlockCake(int id, int textureId) : base(id, textureId, Material.Cake) => setTickRandomly(true);

    public override void updateBoundingBox(IBlockReader iBlockReader, int x, int y, int z)
    {
        int slicesEaten = iBlockReader.GetMeta(x, y, z);
        float edgeInset = 1.0F / 16.0F;
        float minX = (1 + slicesEaten * 2) / 16.0F;
        float height = 0.5F;
        setBoundingBox(minX, 0.0F, edgeInset, 1.0F - edgeInset, height, 1.0F - edgeInset);
    }

    public override void setupRenderBoundingBox()
    {
        float edgeInset = 1.0F / 16.0F;
        float height = 0.5F;
        setBoundingBox(edgeInset, 0.0F, edgeInset, 1.0F - edgeInset, height, 1.0F - edgeInset);
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        int slicesEaten = world.GetMeta(x, y, z);
        float edgeInset = 1.0F / 16.0F;
        float minX = (1 + slicesEaten * 2) / 16.0F;
        float height = 0.5F;
        return new Box(x + minX, y, z + edgeInset, x + 1 - edgeInset, y + height - edgeInset, z + 1 - edgeInset);
    }

    public override Box getBoundingBox(IBlockReader world, int x, int y, int z)
    {
        int slicesEaten = world.GetMeta(x, y, z);
        float edgeInset = 1.0F / 16.0F;
        float minX = (1 + slicesEaten * 2) / 16.0F;
        float height = 0.5F;
        return new Box(x + minX, y, z + edgeInset, x + 1 - edgeInset, y + height, z + 1 - edgeInset);
    }

    public override int getTexture(int side, int meta) => side == 1 ? textureId : side == 0 ? textureId + 3 : meta > 0 && side == 4 ? textureId + 2 : textureId + 1;

    public override int getTexture(int side) => side == 1 ? textureId : side == 0 ? textureId + 3 : textureId + 1;

    public override bool isFullCube() => false;

    public override bool isOpaque() => false;

    public override bool onUse(OnUseEvt evt)
    {
        if (evt.Player.health < 20)
        {
            evt.Player.heal(3);
            int slicesEaten = evt.Level.BlocksReader.GetMeta(evt.X, evt.Y, evt.Z) + 1;
            if (slicesEaten >= 6)
            {
                evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
            }
            else
            {
                evt.Level.BlockWriter.SetBlockMeta(evt.X, evt.Y, evt.Z, slicesEaten);
                evt.Level.Broadcaster.SetBlocksDirty(evt.X, evt.Y, evt.Z);
            }
        }

        return true;
    }

    public override void onBlockBreakStart(OnBlockBreakStartEvt evt)
    {
        if (evt.Player.health >= 20)
        {
            return;
        }

        evt.Player.heal(3);
        int slicesEaten = evt.Level.BlocksReader.GetMeta(evt.X, evt.Y, evt.Z) + 1;
        if (slicesEaten >= 6)
        {
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
        }
        else
        {
            evt.Level.BlockWriter.SetBlockMeta(evt.X, evt.Y, evt.Z, slicesEaten);
            evt.Level.Broadcaster.SetBlocksDirty(evt.X, evt.Y, evt.Z);
        }
    }

    public override bool canPlaceAt(CanPlaceAtCtx evt) => !base.canPlaceAt(evt) ? false : canGrow(evt.Level.BlocksReader, evt.X, evt.Y, evt.Z);

    public override void neighborUpdate(OnTickEvt evt)
    {
        if (canGrow(evt.Level.BlocksReader, evt.X, evt.Y, evt.Z))
        {
            return;
        }

        dropStacks(new OnDropEvt(evt.Level, evt.X, evt.Y, evt.Z, evt.Level.BlocksReader.GetMeta(evt.X, evt.Y, evt.Z)));
        evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
    }

    public override bool canGrow(OnTickEvt evt) => canGrow(evt.Level.BlocksReader, evt.X, evt.Y, evt.Z);

    private static bool canGrow(IBlockReader world, int x, int y, int z) => world.GetMaterial(x, y - 1, z).IsSolid;

    public override int getDroppedItemCount() => 0;

    public override int getDroppedItemId(int blockMeta) => 0;
}
