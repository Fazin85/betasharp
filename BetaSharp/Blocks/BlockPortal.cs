using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

public class BlockPortal : BlockBreakable
{
    public BlockPortal(int id, int textureId) : base(id, textureId, Material.NetherPortal, false)
    {
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z) => null;

    public override void updateBoundingBox(IBlockReader iBlockReader, int x, int y, int z)
    {
        float thickness;
        float halfExtent;
        if (iBlockReader.GetBlockId(x - 1, y, z) != id && iBlockReader.GetBlockId(x + 1, y, z) != id)
        {
            thickness = 2.0F / 16.0F;
            halfExtent = 0.5F;
            setBoundingBox(0.5F - thickness, 0.0F, 0.5F - halfExtent, 0.5F + thickness, 1.0F, 0.5F + halfExtent);
        }
        else
        {
            thickness = 0.5F;
            halfExtent = 2.0F / 16.0F;
            setBoundingBox(0.5F - thickness, 0.0F, 0.5F - halfExtent, 0.5F + thickness, 1.0F, 0.5F + halfExtent);
        }
    }

    public override bool isOpaque() => false;

    public override bool isFullCube() => false;

    public bool create(IBlockReader reader, IBlockWrite writer, int x, int y, int z)
    {
        sbyte extendsInZ = 0;
        sbyte extendsInX = 0;
        if (reader.GetBlockId(x - 1, y, z) == Obsidian.id || reader.GetBlockId(x + 1, y, z) == Obsidian.id)
        {
            extendsInZ = 1;
        }

        if (reader.GetBlockId(x, y, z - 1) == Obsidian.id || reader.GetBlockId(x, y, z + 1) == Obsidian.id)
        {
            extendsInX = 1;
        }

        if (extendsInZ == extendsInX)
        {
            return false;
        }

        if (reader.GetBlockId(x - extendsInZ, y, z - extendsInX) == 0)
        {
            x -= extendsInZ;
            z -= extendsInX;
        }

        int horizontalOffset;
        int verticalOffset;
        for (horizontalOffset = -1; horizontalOffset <= 2; ++horizontalOffset)
        {
            for (verticalOffset = -1; verticalOffset <= 3; ++verticalOffset)
            {
                bool isFrame = horizontalOffset == -1 || horizontalOffset == 2 || verticalOffset == -1 || verticalOffset == 3;
                if ((horizontalOffset != -1 && horizontalOffset != 2) || (verticalOffset != -1 && verticalOffset != 3))
                {
                    int blockId = reader.GetBlockId(x + extendsInZ * horizontalOffset, y + verticalOffset, z + extendsInX * horizontalOffset);
                    if (isFrame)
                    {
                        if (blockId != Obsidian.id)
                        {
                            return false;
                        }
                    }
                    else if (blockId != 0 && blockId != Fire.id)
                    {
                        return false;
                    }
                }
            }
        }

        // TODO: Need to check this PauseTicking stuff
        //ctx.Level.PauseTicking = true;

        for (horizontalOffset = 0; horizontalOffset < 2; ++horizontalOffset)
        {
            for (verticalOffset = 0; verticalOffset < 3; ++verticalOffset)
            {
                writer.SetBlock(x + extendsInZ * horizontalOffset, y + verticalOffset, z + extendsInX * horizontalOffset, NetherPortal.id);
            }
        }

        // TODO: Need to check this PauseTicking stuff
        //ctx.Level.PauseTicking = false;
        return true;
    }

    public override void neighborUpdate(OnTickEvt evt)
    {
        sbyte offsetX = 0;
        sbyte offsetZ = 1;
        if (evt.Level.BlocksReader.GetBlockId(evt.X - 1, evt.Y, evt.Z) == id || evt.Level.BlocksReader.GetBlockId(evt.X + 1, evt.Y, evt.Z) == id)
        {
            offsetX = 1;
            offsetZ = 0;
        }

        int portalBottomY;
        for (portalBottomY = evt.Y; evt.Level.BlocksReader.GetBlockId(evt.X, portalBottomY - 1, evt.Z) == id; --portalBottomY)
        {
        }

        if (evt.Level.BlocksReader.GetBlockId(evt.X, portalBottomY - 1, evt.Z) != Obsidian.id)
        {
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
        }
        else
        {
            int blocksAbove;
            for (blocksAbove = 1; blocksAbove < 4 && evt.Level.BlocksReader.GetBlockId(evt.X, portalBottomY + blocksAbove, evt.Z) == id; ++blocksAbove)
            {
            }

            if (blocksAbove == 3 && evt.Level.BlocksReader.GetBlockId(evt.X, portalBottomY + blocksAbove, evt.Z) == Obsidian.id)
            {
                bool hasXNeighbors = evt.Level.BlocksReader.GetBlockId(evt.X - 1, evt.Y, evt.Z) == id || evt.Level.BlocksReader.GetBlockId(evt.X + 1, evt.Y, evt.Z) == id;
                bool hasZNeighbors = evt.Level.BlocksReader.GetBlockId(evt.X, evt.Y, evt.Z - 1) == id || evt.Level.BlocksReader.GetBlockId(evt.X, evt.Y, evt.Z + 1) == id;
                if (hasXNeighbors && hasZNeighbors)
                {
                    evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
                }
                else if ((evt.Level.BlocksReader.GetBlockId(evt.X + offsetX, evt.Y, evt.Z + offsetZ) != Obsidian.id || evt.Level.BlocksReader.GetBlockId(evt.X - offsetX, evt.Y, evt.Z - offsetZ) != id) &&
                         (evt.Level.BlocksReader.GetBlockId(evt.X - offsetX, evt.Y, evt.Z - offsetZ) != Obsidian.id || evt.Level.BlocksReader.GetBlockId(evt.X + offsetX, evt.Y, evt.Z + offsetZ) != id))
                {
                    evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
                }
            }
            else
            {
                evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
            }
        }
    }

    public override bool isSideVisible(IBlockReader iBlockReader, int x, int y, int z, int side)
    {
        if (iBlockReader.GetBlockId(x, y, z) == id)
        {
            return false;
        }

        bool edgeWest = iBlockReader.GetBlockId(x - 1, y, z) == id && iBlockReader.GetBlockId(x - 2, y, z) != id;
        bool edgeEast = iBlockReader.GetBlockId(x + 1, y, z) == id && iBlockReader.GetBlockId(x + 2, y, z) != id;
        bool edgeNorth = iBlockReader.GetBlockId(x, y, z - 1) == id && iBlockReader.GetBlockId(x, y, z - 2) != id;
        bool edgeSouth = iBlockReader.GetBlockId(x, y, z + 1) == id && iBlockReader.GetBlockId(x, y, z + 2) != id;
        bool extendsInX = edgeWest || edgeEast;
        bool extendsInZ = edgeNorth || edgeSouth;
        return extendsInX && side == 4 ? true : extendsInX && side == 5 ? true : extendsInZ && side == 2 ? true : extendsInZ && side == 3;
    }

    public override int getDroppedItemCount() => 0;

    public override int getRenderLayer() => 1;

    public override void onEntityCollision(OnEntityCollisionEvt evt)
    {
        if (evt.Entity.vehicle == null && evt.Entity.passenger == null)
        {
            evt.Entity.tickPortalCooldown();
        }
    }

    public override void randomDisplayTick(OnTickEvt evt)
    {
        if (Random.Shared.Next(100) == 0)
        {
            evt.Level.Broadcaster.PlaySoundAtPos(evt.X + 0.5D, evt.Y + 0.5D, evt.Z + 0.5D, "portal.portal", 1.0F, Random.Shared.NextSingle() * 0.4F + 0.8F);
        }

        for (int particleIndex = 0; particleIndex < 4; ++particleIndex)
        {
            double particleX = evt.X + Random.Shared.NextSingle();
            double particleY = evt.Y + Random.Shared.NextSingle();
            double particleZ = evt.Z + Random.Shared.NextSingle();
            int direction = Random.Shared.Next(2) * 2 - 1;
            double velocityX = (Random.Shared.NextSingle() - 0.5D) * 0.5D;
            double velocityY = (Random.Shared.NextSingle() - 0.5D) * 0.5D;
            double velocityZ = (Random.Shared.NextSingle() - 0.5D) * 0.5D;
            if (evt.Level.BlocksReader.GetBlockId(evt.X - 1, evt.Y, evt.Z) != id && evt.Level.BlocksReader.GetBlockId(evt.X + 1, evt.Y, evt.Z) != id)
            {
                particleX = evt.X + 0.5D + 0.25D * direction;
                velocityX = Random.Shared.NextSingle() * 2.0F * direction;
            }
            else
            {
                particleZ = evt.Z + 0.5D + 0.25D * direction;
                velocityZ = Random.Shared.NextSingle() * 2.0F * direction;
            }

            evt.Level.Broadcaster.AddParticle("portal", particleX, particleY, particleZ, velocityX, velocityY, velocityZ);
        }
    }
}
