using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockBed : Block
{
    public static readonly int[][] BedOffests = [[0, 1], [-1, 0], [0, -1], [1, 0]];

    public BlockBed(int id) : base(id, 134, Material.Wool)
    {
        setDefaultShape();
    }

    public override bool onUse(World World, int X, int Y, int Z, EntityPlayer Player)
    {
        if (World.isRemote)
        {
            return true;
        }
        else
        {
            int meta = World.getBlockMeta(X, Y, Z);
            if (!isHeadOfBed(meta))
            {
                int direction = getDirection(meta);
                int x = X + BedOffests[direction][0];
                int z = Z + BedOffests[direction][1];
                if (World.getBlockId(x, Y, z) != id)
                {
                    return true;
                }

                meta = World.getBlockMeta(x, Y, z);
            }

            if (!World.Dimension.hasWorldSpawn())
            {
                double posX = (double)X + 0.5D;
                double posY = (double)Y + 0.5D;
                double posZ = (double)Z + 0.5D;
                World.setBlock(X, Y, Z, 0);
                int direction = getDirection(meta);
                X += BedOffests[direction][0];
                Z += BedOffests[direction][1];
                if (World.getBlockId(X, Y, Z) == id)
                {
                    World.setBlock(X, Y, Z, 0);
                    posX = (posX + (double)X + 0.5D) / 2.0D;
                    posY = (posY + (double)Y + 0.5D) / 2.0D;
                    posZ = (posZ + (double)Z + 0.5D) / 2.0D;
                }

                World.createExplosion((Entity)null, (double)((float)X + 0.5F), (double)((float)Y + 0.5F), (double)((float)Z + 0.5F), 5.0F, true);
                return true;
            }
            else
            {
                if (isBedOccupied(meta))
                {
                    EntityPlayer occupant = null;
                    foreach (var otherPlayer in World.players) {
                        if (otherPlayer.isSleeping())
                        {
                            Vec3i sleepingPos = otherPlayer.sleepingPos;
                            if (sleepingPos.x == X && sleepingPos.y == Y && sleepingPos.z == Z)
                            {
                                occupant = otherPlayer;
                            }
                        }
                    }

                    if (occupant != null)
                    {
                        Player.sendMessage("tile.bed.occupied");
                        return true;
                    }

                    updateState(World, X, Y, Z, false);
                }

                SleepAttemptResult result = Player.trySleep(X, Y, Z);
                if (result == SleepAttemptResult.OK)
                {
                    updateState(World, X, Y, Z, true);
                    return true;
                }
                else
                {
                    if (result == SleepAttemptResult.NOT_POSSIBLE_NOW)
                    {
                        Player.sendMessage("tile.bed.noSleep");
                    }

                    return true;
                }
            }
        }
    }

    public override int getTexture(int Side, int Meta)
    {
        if (Side == 0)
        {
            return Block.Planks.textureId;
        }
        else
        {
            int direction = getDirection(Meta);
            int sideFacing = Facings.BedFacings[direction][Side];
            return isHeadOfBed(Meta) ?
                (sideFacing == 2 ? textureId + 2 + 16 : (sideFacing != 5 && sideFacing != 4 ? textureId + 1 : textureId + 1 + 16)) :
                (sideFacing == 3 ? textureId - 1 + 16 : (sideFacing != 5 && sideFacing != 4 ? textureId : textureId + 16));
        }
    }

    public override int getRenderType()
    {
        return 14;
    }

    public override bool isFullCube()
    {
        return false;
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override void updateBoundingBox(BlockView BlockView, int X, int Y, int Z)
    {
        setDefaultShape();
    }

    public override void neighborUpdate(World World, int X, int Y, int Z, int id)
    {
        int blockMeta = World.getBlockMeta(X, Y, Z);
        int direction = getDirection(blockMeta);
        if (isHeadOfBed(blockMeta))
        {
            if (World.getBlockId(X - BedOffests[direction][0], Y, Z - BedOffests[direction][1]) != this.id)
            {
                World.setBlock(X, Y, Z, 0);
            }
        }
        else if (World.getBlockId(X + BedOffests[direction][0], Y, Z + BedOffests[direction][1]) != this.id)
        {
            World.setBlock(X, Y, Z, 0);
            if (!World.isRemote)
            {
                dropStacks(World, X, Y, Z, blockMeta);
            }
        }

    }

    public override int getDroppedItemId(int BlockMeta, java.util.Random Random)
    {
        return isHeadOfBed(BlockMeta) ? 0 : Item.BED.id;
    }

    private void setDefaultShape()
    {
        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 9.0F / 16.0F, 1.0F);
    }

    public static int getDirection(int Meta)
    {
        return Meta & 3;
    }

    public static bool isHeadOfBed(int Meta)
    {
        return (Meta & 8) != 0;
    }

    public static bool isBedOccupied(int Meta)
    {
        return (Meta & 4) != 0;
    }

    public static void updateState(World World, int X, int Y, int Z, bool Occupied)
    {
        int BlockMeta = World.getBlockMeta(X, Y, Z);
        if (Occupied)
        {
            BlockMeta |= 4;
        }
        else
        {
            BlockMeta &= -5;
        }

        World.setBlockMeta(X, Y, Z, BlockMeta);
    }

    public static Vec3i findWakeUpPosition(World World, int X, int Y, int Z, int Skip)
    {
        int BlockMeta = World.getBlockMeta(X, Y, Z);
        int direction = getDirection(BlockMeta);

        for (int BedHalf = 0; BedHalf <= 1; ++BedHalf)
        {
            int SearchMinX = X - BedOffests[direction][0] * BedHalf - 1;
            int SearchMinZ = Z - BedOffests[direction][1] * BedHalf - 1;
            int SearchMaxX = SearchMinX + 2;
            int SearchMaxZ = SearchMinZ + 2;

            for (int checkX = SearchMinX; checkX <= SearchMaxX; ++checkX)
            {
                for (int checkZ = SearchMinZ; checkZ <= SearchMaxZ; ++checkZ)
                {
                    if (World.shouldSuffocate(checkX, Y - 1, checkZ) && World.isAir(checkX, Y, checkZ) && World.isAir(checkX, Y + 1, checkZ))
                    {
                        if (Skip <= 0)
                        {
                            return new Vec3i(checkX, Y, checkZ);
                        }

                        --Skip;
                    }
                }
            }
        }

        return null;
    }

    public override void dropStacks(World World, int X, int Y, int Z, int Meta, float Luck)
    {
        if (!isHeadOfBed(Meta))
        {
            base.dropStacks(World, X, Y, Z, Meta, Luck);
        }

    }

    public override int getPistonBehavior()
    {
        return 1;
    }
}