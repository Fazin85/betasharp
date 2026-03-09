using BetaSharp.Blocks.Materials;
using BetaSharp.Rules;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockFire : Block
{
    private readonly int[] _burnChances = new int[256];
    private readonly int[] _spreadChances = new int[256];

    public BlockFire(int id, int textureId) : base(id, textureId, Material.Fire) => setTickRandomly(true);

    protected override void init()
    {
        registerFlammableBlock(Planks.id, 5, 20);
        registerFlammableBlock(Fence.id, 5, 20);
        registerFlammableBlock(WoodenStairs.id, 5, 20);
        registerFlammableBlock(Log.id, 5, 5);
        registerFlammableBlock(Leaves.id, 30, 60);
        registerFlammableBlock(Bookshelf.id, 30, 20);
        registerFlammableBlock(TNT.id, 15, 100);
        registerFlammableBlock(Grass.id, 60, 100);
        registerFlammableBlock(Wool.id, 30, 60);
    }

    private void registerFlammableBlock(int block, int burnChange, int spreadChance)
    {
        _burnChances[block] = burnChange;
        _spreadChances[block] = spreadChance;
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z) => null;

    public override bool isOpaque() => false;

    public override bool isFullCube() => false;

    public override BlockRendererType getRenderType() => BlockRendererType.Fire;

    public override int getDroppedItemCount() => 0;

    public override int getTickRate() => 40;

    public override void onTick(OnTickEvt evt)
    {
        if (!evt.Level.Rules.GetBool(DefaultRules.DoFireTick))
        {
            return;
        }

        bool isOnNetherrack = evt.Level.BlocksReader.GetBlockId(evt.X, evt.Y - 1, evt.Z) == Netherrack.id;
        if (!canPlaceAt(new CanPlaceAtCtx(evt.Level, 0, evt.X, evt.Y, evt.Z)))
        {
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
        }

        if (isOnNetherrack ||
            !evt.Level.Environment.IsRaining ||
            (!evt.Level.Environment.IsRainingAt(evt.X, evt.Y, evt.Z) &&
             !evt.Level.Environment.IsRainingAt(evt.X - 1, evt.Y, evt.Z) &&
             !evt.Level.Environment.IsRainingAt(evt.X + 1, evt.Y, evt.Z) &&
             !evt.Level.Environment.IsRainingAt(evt.X, evt.Y, evt.Z - 1) &&
             !evt.Level.Environment.IsRainingAt(evt.X, evt.Y, evt.Z + 1)))
        {
            int fireAge = evt.Level.BlocksReader.GetMeta(evt.X, evt.Y, evt.Z);
            if (fireAge < 15)
            {
                evt.Level.BlockWriter.SetBlockMetaWithoutNotifyingNeighbors(evt.X, evt.Y, evt.Z, fireAge + evt.Level.random.NextInt(3) / 2);
            }
            evt.Level.TickScheduler.ScheduleBlockUpdate(evt.X, evt.Y, evt.Z, id, getTickRate());
            if (!isOnNetherrack && !areBlocksAroundFlammable(evt.Level.BlocksReader, evt.X, evt.Y, evt.Z))
            {
                if (!evt.Level.BlocksReader.ShouldSuffocate(evt.X, evt.Y - 1, evt.Z) || fireAge > 3)
                {
                    evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
                }
            }
            else if (!isOnNetherrack && !isFlammable(evt.Level.BlocksReader, evt.X, evt.Y - 1, evt.Z) && fireAge == 15 && evt.Level.random.NextInt(4) == 0)
            {
                evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
            }
            else
            {
                trySpreadingFire(evt.Level, evt.X + 1, evt.Y, evt.Z, 300, evt.Level.random, fireAge);
                trySpreadingFire(evt.Level, evt.X - 1, evt.Y, evt.Z, 300, evt.Level.random, fireAge);
                trySpreadingFire(evt.Level, evt.X, evt.Y - 1, evt.Z, 250, evt.Level.random, fireAge);
                trySpreadingFire(evt.Level, evt.X, evt.Y + 1, evt.Z, 250, evt.Level.random, fireAge);
                trySpreadingFire(evt.Level, evt.X, evt.Y, evt.Z - 1, 300, evt.Level.random, fireAge);
                trySpreadingFire(evt.Level, evt.X, evt.Y, evt.Z + 1, 300, evt.Level.random, fireAge);

                for (int checkX = evt.X - 1; checkX <= evt.X + 1; ++checkX)
                {
                    for (int checkY = evt.Z - 1; checkY <= evt.Z + 1; ++checkY)
                    {
                        for (int checkZ = evt.Y - 1; checkZ <= evt.Y + 4; ++checkZ)
                        {
                            if (checkX != evt.X || checkZ != evt.Y || checkY != evt.Z)
                            {
                                int spreadDifficulty = 100;
                                if (checkZ > evt.Y + 1)
                                {
                                    spreadDifficulty += (checkZ - (evt.Y + 1)) * 100;
                                }

                                int burnChance = getBurnChance(evt.Level.BlocksReader, checkX, checkZ, checkY);
                                if (burnChance > 0)
                                {
                                    int var13 = (burnChance + 40) / (fireAge + 30);
                                    if (var13 > 0 &&
                                        evt.Level.random.NextInt(spreadDifficulty) <= var13 &&
                                        (!evt.Level.Environment.IsRaining || !evt.Level.Environment.IsRainingAt(checkX, checkZ, checkY)) &&
                                        !evt.Level.Environment.IsRainingAt(checkX - 1, checkZ, checkY) &&
                                        !evt.Level.Environment.IsRainingAt(checkX + 1, checkZ, checkY) &&
                                        !evt.Level.Environment.IsRainingAt(checkX, checkZ, checkY - 1) &&
                                        !evt.Level.Environment.IsRainingAt(checkX, checkZ, checkY + 1))
                                    {
                                        int spreadChance = fireAge + evt.Level.random.NextInt(5) / 4;
                                        if (spreadChance > 15)
                                        {
                                            spreadChance = 15;
                                        }

                                        evt.Level.BlockWriter.SetBlock(checkX, checkZ, checkY, id, spreadChance);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
        }
    }

    private void trySpreadingFire(IBlockWorldContext level, int x, int y, int z, int spreadFactor, JavaRandom random, int currentAge)
    {
        int targetSpreadChance = _spreadChances[level.BlocksReader.GetBlockId(x, y, z)];
        if (random.NextInt(spreadFactor) < targetSpreadChance)
        {
            bool isTnt = level.BlocksReader.GetBlockId(x, y, z) == TNT.id;
            if (random.NextInt(currentAge + 10) < 5 && !level.Environment.IsRainingAt(x, y, z))
            {
                int newFireAge = currentAge + random.NextInt(5) / 4;
                if (newFireAge > 15)
                {
                    newFireAge = 15;
                }

                level.BlockWriter.SetBlock(x, y, z, id, newFireAge);
            }
            else
            {
                level.BlockWriter.SetBlock(x, y, z, 0);
            }

            if (isTnt)
            {
                TNT.onMetadataChange(new OnMetadataChangeEvt(level, x, y, z, 1));
            }
        }
    }

    private bool areBlocksAroundFlammable(IBlockReader world, int x, int y, int z) => isFlammable(world, x + 1, y, z) ? true :
        isFlammable(world, x - 1, y, z) ? true :
        isFlammable(world, x, y - 1, z) ? true :
        isFlammable(world, x, y + 1, z) ? true :
        isFlammable(world, x, y, z - 1) ? true : isFlammable(world, x, y, z + 1);

    private int getBurnChance(IBlockReader world, int x, int y, int z)
    {
        sbyte initialMax = 0;
        if (!world.IsAir(x, y, z))
        {
            return 0;
        }

        int maxChance = getBurnChance(world, x + 1, y, z, initialMax);
        maxChance = getBurnChance(world, x - 1, y, z, maxChance);
        maxChance = getBurnChance(world, x, y - 1, z, maxChance);
        maxChance = getBurnChance(world, x, y + 1, z, maxChance);
        maxChance = getBurnChance(world, x, y, z - 1, maxChance);
        maxChance = getBurnChance(world, x, y, z + 1, maxChance);
        return maxChance;
    }

    public override bool hasCollision() => false;

    public override bool isFlammable(IBlockReader reader, int x, int y, int z) => _burnChances[reader.GetBlockId(x, y, z)] > 0;

    public int getBurnChance(IBlockReader world, int x, int y, int z, int currentChance)
    {
        int blockBurnChance = _burnChances[world.GetBlockId(x, y, z)];
        return blockBurnChance > currentChance ? blockBurnChance : currentChance;
    }

    public override bool canPlaceAt(CanPlaceAtCtx ctx) => ctx.Level.BlocksReader.ShouldSuffocate(ctx.X, ctx.Y - 1, ctx.Z) || areBlocksAroundFlammable(ctx.Level.BlocksReader, ctx.X, ctx.Y, ctx.Z);

    public override void neighborUpdate(OnTickEvt ctx)
    {
        if (!ctx.Level.BlocksReader.ShouldSuffocate(ctx.X, ctx.Y - 1, ctx.Z) && !areBlocksAroundFlammable(ctx.Level.BlocksReader, ctx.X, ctx.Y, ctx.Z))
        {
            ctx.Level.BlockWriter.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }
    }

    public override void onPlaced(OnPlacedEvt ctx)
    {
        if (ctx.Level.BlocksReader.GetBlockId(ctx.X, ctx.Y - 1, ctx.Z) != Obsidian.id || !NetherPortal.create(ctx.Level.BlocksReader, ctx.Level.BlockWriter, ctx.X, ctx.Y, ctx.Z))
        {
            if (!ctx.Level.BlocksReader.ShouldSuffocate(ctx.X, ctx.Y - 1, ctx.Z) && !areBlocksAroundFlammable(ctx.Level.BlocksReader, ctx.X, ctx.Y, ctx.Z))
            {
                ctx.Level.BlockWriter.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
            }
            else
            {
                ctx.Level.TickScheduler.ScheduleBlockUpdate(ctx.X, ctx.Y, ctx.Z, id, getTickRate());
            }
        }
    }

    public override void randomDisplayTick(OnTickEvt ctx)
    {
        if (ctx.Level.random.NextInt(24) == 0)
        {
            ctx.Level.Broadcaster.PlaySoundAtPos(ctx.X + 0.5F, ctx.Y + 0.5F, ctx.Z + 0.5F, "fire.fire", 1.0F + Random.Shared.NextSingle(), Random.Shared.NextSingle() * 0.7F + 0.3F);
        }

        int particleIndex;
        float particleX;
        float particleY;
        float particleZ;
        if (!ctx.Level.BlocksReader.ShouldSuffocate(ctx.X, ctx.Y - 1, ctx.Z) && !Fire.isFlammable(ctx.Level.BlocksReader, ctx.X, ctx.Y - 1, ctx.Z))
        {
            if (Fire.isFlammable(ctx.Level.BlocksReader, ctx.X - 1, ctx.Y, ctx.Z))
            {
                for (particleIndex = 0; particleIndex < 2; ++particleIndex)
                {
                    particleX = ctx.X + Random.Shared.NextSingle() * 0.1F;
                    particleY = ctx.Y + Random.Shared.NextSingle();
                    particleZ = ctx.Z + Random.Shared.NextSingle();
                    ctx.Level.Broadcaster.AddParticle("largesmoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                }
            }

            if (Fire.isFlammable(ctx.Level.BlocksReader, ctx.X + 1, ctx.Y, ctx.Z))
            {
                for (particleIndex = 0; particleIndex < 2; ++particleIndex)
                {
                    particleX = ctx.X + 1 - Random.Shared.NextSingle() * 0.1F;
                    particleY = ctx.Y + Random.Shared.NextSingle();
                    particleZ = ctx.Z + Random.Shared.NextSingle();
                    ctx.Level.Broadcaster.AddParticle("largesmoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                }
            }

            if (Fire.isFlammable(ctx.Level.BlocksReader, ctx.X, ctx.Y, ctx.Z - 1))
            {
                for (particleIndex = 0; particleIndex < 2; ++particleIndex)
                {
                    particleX = ctx.X + Random.Shared.NextSingle();
                    particleY = ctx.Y + Random.Shared.NextSingle();
                    particleZ = ctx.Z + Random.Shared.NextSingle() * 0.1F;
                    ctx.Level.Broadcaster.AddParticle("largesmoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                }
            }

            if (Fire.isFlammable(ctx.Level.BlocksReader, ctx.X, ctx.Y, ctx.Z + 1))
            {
                for (particleIndex = 0; particleIndex < 2; ++particleIndex)
                {
                    particleX = ctx.X + Random.Shared.NextSingle();
                    particleY = ctx.Y + Random.Shared.NextSingle();
                    particleZ = ctx.Z + 1 - Random.Shared.NextSingle() * 0.1F;
                    ctx.Level.Broadcaster.AddParticle("largesmoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                }
            }

            if (Fire.isFlammable(ctx.Level.BlocksReader, ctx.X, ctx.Y + 1, ctx.Z))
            {
                for (particleIndex = 0; particleIndex < 2; ++particleIndex)
                {
                    particleX = ctx.X + Random.Shared.NextSingle();
                    particleY = ctx.Y + 1 - Random.Shared.NextSingle() * 0.1F;
                    particleZ = ctx.Z + Random.Shared.NextSingle();
                    ctx.Level.Broadcaster.AddParticle("largesmoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                }
            }
        }
        else
        {
            for (particleIndex = 0; particleIndex < 3; ++particleIndex)
            {
                particleX = ctx.X + Random.Shared.NextSingle();
                particleY = ctx.Y + Random.Shared.NextSingle() * 0.5F + 0.5F;
                particleZ = ctx.Z + Random.Shared.NextSingle();
                ctx.Level.Broadcaster.AddParticle("largesmoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
            }
        }
    }
}
