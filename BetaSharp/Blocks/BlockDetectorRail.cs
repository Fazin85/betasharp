using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockDetectorRail : BlockRail
{
    public BlockDetectorRail(int id, int textureId) : base(id, textureId, true) => setTickRandomly(true);

    public override int getTickRate() => 20;

    public override bool canEmitRedstonePower() => true;

    public override void onEntityCollision(OnEntityCollisionEvt ctx)
    {
        if (!ctx.IsRemote)
        {
            int meta = ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
            if ((meta & 8) == 0)
            {
                updatePoweredStatus(ctx.Entities, ctx.WorldWrite, ctx.Broadcaster, ctx.X, ctx.Y, ctx.Z, id, meta);
            }
        }
    }

    public override void onTick(OnTickEvt ctx)
    {
        if (!ctx.IsRemote)
        {
            int meta = ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
            if ((meta & 8) != 0)
            {
                updatePoweredStatus(ctx.Entities, ctx.WorldWrite, ctx.Broadcaster, ctx.X, ctx.Y, ctx.Z, id, meta);
            }
        }
    }

    public override bool isPoweringSide(IBlockReader iBlockReader, int x, int y, int z, int side) => (iBlockReader.GetBlockMeta(x, y, z) & 8) != 0;

    public override bool isStrongPoweringSide(IBlockReader world, int x, int y, int z, int side) => (world.GetBlockMeta(x, y, z) & 8) == 0 ? false : side == 1;

    private void updatePoweredStatus(        EntityManager entities,        IBlockWrite worldWrite,  WorldEventBroadcaster broadcaster,      int x,        int y,        int z,        int id        , int meta)
    {
        bool isPowered = (meta & 8) != 0;
        bool hasMinecart = false;
        float detectionInset = 2.0F / 16.0F;
        List<EntityMinecart> minecartsOnRail = entities.CollectEntitiesOfType<EntityMinecart>(new Box(x + detectionInset, y, z + detectionInset, x + 1 - detectionInset, y + 0.25D, z + 1 - detectionInset));
        if (minecartsOnRail.Count > 0)
        {
            hasMinecart = true;
        }

        if (hasMinecart && !isPowered)
        {
            worldWrite.SetBlockMeta(x, y, z, meta | 8);
            broadcaster.NotifyNeighbors(x, y, z, id);
            broadcaster.NotifyNeighbors(x, y - 1, z, id);
            worldWrite.SetBlocksDirty(x, y, z, x, y, z);
        }

        if (!hasMinecart && isPowered)
        {
            worldWrite.SetBlockMeta(x, y, z, meta & 7);
            broadcaster.NotifyNeighbors(x, y, z, id);
            broadcaster.NotifyNeighbors(x, y - 1, z, id);
            worldWrite.SetBlocksDirty(x, y, z, x, y, z);
        }

        if (hasMinecart)
        {
            // TODO: Implement this
            //broadcaster.ScheduleBlockUpdate(x, y, z, id, getTickRate());
        }
    }
}
