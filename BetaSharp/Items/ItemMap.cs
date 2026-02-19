using System;
using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Network.Packets;
using BetaSharp.Network.Packets.S2CPlay;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Storage; // Added for PersistentState

namespace BetaSharp.Items;

public class ItemMap : NetworkSyncedItem
{
    public ItemMap(int id) : base(id)
    {
        setMaxCount(1);
    }

    public static MapState GetMapState(short mapId, World world)
    {
        string mapName = $"map_{mapId}";
        // Using the new Generic method:
        MapState mapState = world.GetOrCreateState<MapState>(mapName);

        if (mapState == null)
        {
            int nextId = world.getIdCount("map");
            string newName = $"map_{nextId}";
            mapState = new MapState(newName);
            world.setState(newName, mapState);
        }

        return mapState;
    }

    public MapState GetSavedMapState(ItemStack stack, World world)
    {
        string mapName = $"map_{stack.getDamage()}";
        // Using the new Generic method:
        MapState mapState = world.GetOrCreateState<MapState>(mapName);

        if (mapState == null)
        {
            stack.setDamage(world.getIdCount("map"));
            string newName = $"map_{stack.getDamage()}";

            mapState = new MapState(newName)
            {
                centerX = world.getProperties().SpawnX,
                centerZ = world.getProperties().SpawnZ,
                scale = 3,
                dimension = (sbyte)world.dimension.id
            };

            mapState.markDirty();
            world.setState(newName, mapState);
        }

        return mapState;
    }

    public void UpdateMap(World world, Entity entity, MapState map)
    {
        if (world.dimension.id != map.dimension) return;

        const int mapWidth = 128;
        const int mapHeight = 128;
        int blocksPerPixel = 1 << map.scale;
        int centerX = map.centerX;
        int centerZ = map.centerZ;

        int entityPosX = MathHelper.floor_double(entity.x - centerX) / blocksPerPixel + mapWidth / 2;
        int entityPosZ = MathHelper.floor_double(entity.z - centerZ) / blocksPerPixel + mapHeight / 2;
        int scanRadius = 128 / blocksPerPixel;

        if (world.dimension.hasCeiling)
        {
            scanRadius /= 2;
        }

        map.inventoryTicks++;

        for (int pixelX = entityPosX - scanRadius + 1; pixelX < entityPosX + scanRadius; ++pixelX)
        {
            if ((pixelX & 15) == (map.inventoryTicks & 15))
            {
                int minDirtyZ = 255;
                int maxDirtyZ = 0;
                double lastHeight = 0.0D;

                for (int pixelZ = entityPosZ - scanRadius - 1; pixelZ < entityPosZ + scanRadius; ++pixelZ)
                {
                    if (pixelX >= 0 && pixelZ >= -1 && pixelX < mapWidth && pixelZ < mapHeight)
                    {
                        int dx = pixelX - entityPosX;
                        int dy = pixelZ - entityPosZ;
                        bool isOutside = dx * dx + dy * dy > (scanRadius - 2) * (scanRadius - 2);

                        int worldX = (centerX / blocksPerPixel + pixelX - mapWidth / 2) * blocksPerPixel;
                        int worldZ = (centerZ / blocksPerPixel + pixelZ - mapHeight / 2) * blocksPerPixel;

                        int[] blockHistogram = new int[256];
                        Chunk chunk = world.getChunkFromPos(worldX, worldZ);

                        int chunkOffsetX = worldX & 15;
                        int chunkOffsetZ = worldZ & 15;
                        int fluidDepth = 0;
                        double avgHeight = 0.0D;
                        int sampleX;
                        int sampleZ;

                        if (world.dimension.hasCeiling)
                        {
                            int hash = worldX + worldZ * 231871;
                            hash = hash * hash * 31287121 + hash * 11;

                            if ((hash >> 20 & 1) == 0) blockHistogram[Block.Dirt.id] += 10;
                            else blockHistogram[Block.Stone.id] += 10;

                            avgHeight = 100.0D;
                        }
                        else
                        {
                            for (sampleX = 0; sampleX < blocksPerPixel; ++sampleX)
                            {
                                for (sampleZ = 0; sampleZ < blocksPerPixel; ++sampleZ)
                                {
                                    int currentY = chunk.getHeight(sampleX + chunkOffsetX, sampleZ + chunkOffsetZ) + 1;
                                    int blockId = 0;

                                    if (currentY > 1)
                                    {
                                        ProcessBlockHeight(chunk, sampleX, chunkOffsetX, sampleZ, chunkOffsetZ, ref currentY, out blockId, ref fluidDepth);
                                    }

                                    avgHeight += (double)currentY / (blocksPerPixel * blocksPerPixel);
                                    blockHistogram[blockId]++;
                                }
                            }
                        }

                        fluidDepth /= (blocksPerPixel * blocksPerPixel);
                        sampleX = 0; // Max frequency
                        sampleZ = 0; // Most common Block ID

                        for (int i = 0; i < 256; ++i)
                        {
                            if (blockHistogram[i] > sampleX)
                            {
                                sampleZ = i;
                                sampleX = blockHistogram[i];
                            }
                        }

                        double shadeFactor = (avgHeight - lastHeight) * 4.0D / (blocksPerPixel + 4) + ((double)(pixelX + pixelZ & 1) - 0.5D) * 0.4D;
                        byte brightness = (byte)(shadeFactor > 0.6D ? 2 : (shadeFactor < -0.6D ? 0 : 1));

                        int colorIndex = 0;
                        if (sampleZ > 0)
                        {
                            MapColor mapColor = Block.Blocks[sampleZ].material.MapColor;
                            if (mapColor == MapColor.waterColor)
                            {
                                shadeFactor = fluidDepth * 0.1D + (double)(pixelX + pixelZ & 1) * 0.2D;
                                brightness = (byte)(shadeFactor < 0.5D ? 2 : (shadeFactor > 0.9D ? 0 : 1));
                            }
                            colorIndex = mapColor.colorIndex;
                        }

                        lastHeight = avgHeight;
                        if (pixelZ >= 0 && dx * dx + dy * dy < scanRadius * scanRadius && (!isOutside || (pixelX + pixelZ & 1) != 0))
                        {
                            byte currentColor = map.colors[pixelX + pixelZ * mapWidth];
                            byte pixelColor = (byte)(colorIndex * 4 + brightness);

                            if (currentColor != pixelColor)
                            {
                                if (minDirtyZ > pixelZ) minDirtyZ = pixelZ;
                                if (maxDirtyZ < pixelZ) maxDirtyZ = pixelZ;
                                map.colors[pixelX + pixelZ * mapWidth] = pixelColor;
                            }
                        }
                    }
                }

                if (minDirtyZ <= maxDirtyZ)
                {
                    map.markDirty(pixelX, minDirtyZ, maxDirtyZ);
                }
            }
        }
    }

    private void ProcessBlockHeight(Chunk chunk, int chunkX, int dx, int chunkZ, int dz, ref int scanY, out int blockId, ref int fluidDepth)
    {
        blockId = 0;
        bool exitLoop = false;

        while (!exitLoop)
        {
            bool foundSurface = true;
            blockId = chunk.getBlockId(chunkX + dx, scanY - 1, chunkZ + dz);

            if (blockId == 0 || (scanY > 0 && Block.Blocks[blockId].material.MapColor == MapColor.airColor))
            {
                foundSurface = false;
            }

            if (!foundSurface)
            {
                --scanY;
                blockId = chunk.getBlockId(chunkX + dx, scanY - 1, chunkZ + dz);
            }

            if (foundSurface)
            {
                if (blockId == 0 || !Block.Blocks[blockId].material.IsFluid)
                {
                    exitLoop = true;
                }
                else
                {
                    int depthCheckY = scanY - 1;
                    while (true)
                    {
                        int fluidBlockId = chunk.getBlockId(chunkX + dx, depthCheckY--, chunkZ + dz);
                        ++fluidDepth;
                        if (depthCheckY <= 0 || fluidBlockId == 0 || !Block.Blocks[fluidBlockId].material.IsFluid)
                        {
                            exitLoop = true;
                            break;
                        }
                    }
                }
            }
            if (scanY <= 0) exitLoop = true; // Safety break
        }
    }

    public override void inventoryTick(ItemStack itemStack, World world, Entity entity, int slotIndex, bool shouldUpdate)
    {
        if (world.isRemote) return;

        MapState mapState = GetSavedMapState(itemStack, world);
        if (entity is EntityPlayer player)
        {
            mapState.update(player, itemStack);
        }

        if (shouldUpdate)
        {
            UpdateMap(world, entity, mapState);
        }
    }

    public override void onCraft(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
        int mapId = world.getIdCount("map");
        itemStack.setDamage(mapId);
        string mapName = $"map_{mapId}";

        MapState mapState = new MapState(mapName)
        {
            centerX = MathHelper.floor_double(entityPlayer.x),
            centerZ = MathHelper.floor_double(entityPlayer.z),
            scale = 3,
            dimension = (sbyte)world.dimension.id
        };

        world.setState(mapName, mapState);
        mapState.markDirty();
    }

    public override Packet getUpdatePacket(ItemStack stack, World world, EntityPlayer player)
    {
        byte[] updateData = GetSavedMapState(stack, world).getPlayerMarkerPacket(player);
        return updateData == null ? null : new MapUpdateS2CPacket((short)Item.Map.id, (short)stack.getDamage(), updateData);
    }
}
