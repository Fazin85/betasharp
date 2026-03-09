using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Core.Systems;

public class WorldTickScheduler
{
    private readonly IBlockWorldContext _context;
    private readonly PriorityQueue<BlockUpdate, (long, long)> _scheduledUpdates = new();
    private long _absoluteTickCounter;

    public WorldTickScheduler(IBlockWorldContext context)
    {
        _context = context;
    }
    public void Tick(bool forceFlush = false)
    {
        _absoluteTickCounter++;
        ProcessScheduledTicks(forceFlush);
    }

    public virtual void ScheduleBlockUpdate(int x, int y, int z, int blockId, int tickRate,bool instantBlockUpdateEnabled = false)
    {
        const byte loadRadius = 8;
        if (_context.BlockHost.IsPosLoaded(x - loadRadius, y - loadRadius, z - loadRadius) && _context.BlockHost.IsPosLoaded(x + loadRadius, y + loadRadius, z + loadRadius))
        {
            if (instantBlockUpdateEnabled)
            {
                int currentBlockId = _context.BlocksReader.GetBlockId(x, y, z);
                if (currentBlockId == blockId && currentBlockId > 0)
                {
                    int meta = _context.BlocksReader.GetMeta(x, y, z);
                    Block.Blocks[currentBlockId].onTick(new OnTickEvt(_context, x, y, z, meta, currentBlockId));
                }
            }
            else
            {
                long scheduledTime = _context.GetTime() + tickRate;
                BlockUpdate blockUpdate = new(x, y, z, blockId, scheduledTime);
                _scheduledUpdates.Enqueue(blockUpdate, (blockUpdate.ScheduledTime, blockUpdate.ScheduledOrder));
            }
        }
    }

    private void ProcessScheduledTicks(bool forceFlush)
    {
        if (_context.IsRemote)
        {
            return;
        }

        for (int i = 0; i < 1000; ++i)
        {
            if (_scheduledUpdates.Count == 0)
            {
                break;
            }

            if (!forceFlush && _scheduledUpdates.Peek().ScheduledTime > _absoluteTickCounter)
            {
                break;
            }

            BlockUpdate blockUpdate = _scheduledUpdates.Dequeue();

            const byte loadRadius = 8;
            if (_context.BlocksReader.IsPosLoaded(blockUpdate.X - loadRadius, blockUpdate.Y - loadRadius, blockUpdate.Z - loadRadius) &&
             _context.BlocksReader.IsPosLoaded(blockUpdate.X + loadRadius, blockUpdate.Y + loadRadius, blockUpdate.Z + loadRadius))
            {
                int currentBlockId = _context.BlocksReader.GetBlockId(blockUpdate.X, blockUpdate.Y, blockUpdate.Z);
                if (currentBlockId == blockUpdate.BlockId && currentBlockId > 0)
                {
                    Block.Blocks[currentBlockId].onTick(new OnTickEvt(_context, blockUpdate.X, blockUpdate.Y, blockUpdate.Z, _context.BlocksReader.GetMeta(blockUpdate.X, blockUpdate.Y, blockUpdate.Z), currentBlockId));
                }
            }
        }
    }
}
