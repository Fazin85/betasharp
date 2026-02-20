using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Gen.Carvers;

public class CaveCarver : Carver
{

    protected void CarveCavesInChunk(int chunkX, int chunkZ, byte[] blocks, double offsetX, double offsetY, double offsetZ)
    {
        CarveCaves(chunkX, chunkZ, blocks, offsetX, offsetY, offsetZ, 1.0F + rand.NextFloat() * 6.0F, 0.0F, 0.0F, -1, -1, 0.5D);
    }

    protected void CarveCaves(int chunkX, int chunkZ, byte[] blocks, double offsetX, double offsetY, double offsetZ, float tunnelRadius, float var11, float carvePitch, int tunnelStep, int tunnelLength, double var15)
    {
        double chunkCenterX = chunkX * 16 + 8;
        double chunkCenterZ = chunkZ * 16 + 8;
        float var21 = 0.0F;
        float var22 = 0.0F;
        JavaRandom caveRand = new(rand.NextLong());
        if (tunnelLength <= 0)
        {
            int var24 = radius * 16 - 16;
            tunnelLength = var24 - caveRand.NextInt(var24 / 4);
        }

        bool var52 = false;
        if (tunnelStep == -1)
        {
            tunnelStep = tunnelLength / 2;
            var52 = true;
        }

        int var25 = caveRand.NextInt(tunnelLength / 2) + tunnelLength / 4;

        for (bool var26 = caveRand.NextInt(6) == 0; tunnelStep < tunnelLength; ++tunnelStep)
        {
            double var27 = 1.5D + (double)(MathHelper.sin(tunnelStep * (float)Math.PI / tunnelLength) * tunnelRadius * 1.0F);
            double var29 = var27 * var15;
            float var31 = MathHelper.cos(carvePitch);
            float var32 = MathHelper.sin(carvePitch);
            offsetX += (double)(MathHelper.cos(var11) * var31);
            offsetY += (double)var32;
            offsetZ += (double)(MathHelper.sin(var11) * var31);
            if (var26)
            {
                carvePitch *= 0.92F;
            }
            else
            {
                carvePitch *= 0.7F;
            }

            carvePitch += var22 * 0.1F;
            var11 += var21 * 0.1F;
            var22 *= 0.9F;
            var21 *= 12.0F / 16.0F;
            var22 += (caveRand.NextFloat() - caveRand.NextFloat()) * caveRand.NextFloat() * 2.0F;
            var21 += (caveRand.NextFloat() - caveRand.NextFloat()) * caveRand.NextFloat() * 4.0F;
            if (!var52 && tunnelStep == var25 && tunnelRadius > 1.0F)
            {
                CarveCaves(chunkX, chunkZ, blocks, offsetX, offsetY, offsetZ, caveRand.NextFloat() * 0.5F + 0.5F, var11 - (float)Math.PI * 0.5F, carvePitch / 3.0F, tunnelStep, tunnelLength, 1.0D);
                CarveCaves(chunkX, chunkZ, blocks, offsetX, offsetY, offsetZ, caveRand.NextFloat() * 0.5F + 0.5F, var11 + (float)Math.PI * 0.5F, carvePitch / 3.0F, tunnelStep, tunnelLength, 1.0D);
                return;
            }

            if (var52 || caveRand.NextInt(4) != 0)
            {
                double var33 = offsetX - chunkCenterX;
                double var35 = offsetZ - chunkCenterZ;
                double var37 = tunnelLength - tunnelStep;
                double var39 = (double)(tunnelRadius + 2.0F + 16.0F);
                if (var33 * var33 + var35 * var35 - var37 * var37 > var39 * var39)
                {
                    return;
                }

                if (offsetX >= chunkCenterX - 16.0D - var27 * 2.0D && offsetZ >= chunkCenterZ - 16.0D - var27 * 2.0D && offsetX <= chunkCenterX + 16.0D + var27 * 2.0D && offsetZ <= chunkCenterZ + 16.0D + var27 * 2.0D)
                {
                    int xMin = MathHelper.floor_double(offsetX - var27) - chunkX * 16 - 1;
                    int xMax = MathHelper.floor_double(offsetX + var27) - chunkX * 16 + 1;
                    int yMin = MathHelper.floor_double(offsetY - var29) - 1;
                    int yMax = MathHelper.floor_double(offsetY + var29) + 1;
                    int zMin = MathHelper.floor_double(offsetZ - var27) - chunkZ * 16 - 1;
                    int zMax = MathHelper.floor_double(offsetZ + var27) - chunkZ * 16 + 1;
                    if (xMin < 0)
                    {
                        xMin = 0;
                    }

                    if (xMax > 16)
                    {
                        xMax = 16;
                    }

                    if (yMin < 1)
                    {
                        yMin = 1;
                    }

                    if (yMax > 120)
                    {
                        yMax = 120;
                    }

                    if (zMin < 0)
                    {
                        zMin = 0;
                    }

                    if (zMax > 16)
                    {
                        zMax = 16;
                    }

                    bool waterIsPresent = false;

                    for (int blockX = xMin; !waterIsPresent && blockX < xMax; ++blockX)
                    {
                        for (int blockZ = zMin; !waterIsPresent && blockZ < zMax; ++blockZ)
                        {
                            for (int blockY = yMax + 1; !waterIsPresent && blockY >= yMin - 1; --blockY)
                            {
                                int blockIndex = (blockX * 16 + blockZ) * 128 + blockY;
                                if (blockY >= 0 && blockY < 128)
                                {
                                    if (blocks[blockZ] == Block.FlowingWater.id || blocks[blockZ] == Block.Water.id)
                                    {
                                        waterIsPresent = true;
                                    }

                                    if (blockY != yMin - 1 && blockX != xMin && blockX != xMax - 1 && blockZ != zMin && blockZ != zMax - 1)
                                    {
                                        blockY = yMin;
                                    }
                                }
                            }
                        }
                    }

                    if (!waterIsPresent)
                    {
                        for (int blockX = xMin; blockX < xMax; ++blockX)
                        {
                            double var57 = (blockX + chunkX * 16 + 0.5D - offsetX) / var27;

                            for (int blockZ = zMin; blockZ < zMax; ++blockZ)
                            {
                                double var44 = (blockZ + chunkZ * 16 + 0.5D - offsetZ) / var27;
                                int blockIndex = (blockX * 16 + blockZ) * 128 + yMax;
                                bool isGrassBlock = false;
                                if (var57 * var57 + var44 * var44 < 1.0D)
                                {
                                    for (int var48 = yMax - 1; var48 >= yMin; --var48)
                                    {
                                        double var49 = (var48 + 0.5D - offsetY) / var29;
                                        if (var49 > -0.7D && var57 * var57 + var49 * var49 + var44 * var44 < 1.0D)
                                        {
                                            byte blockType = blocks[blockIndex];
                                            if (blockType == Block.GrassBlock.id)
                                            {
                                                isGrassBlock = true;
                                            }

                                            if (blockType == Block.Stone.id || blockType == Block.Dirt.id || blockType == Block.GrassBlock.id)
                                            {
                                                if (var48 < 10)
                                                {
                                                    blocks[blockIndex] = (byte)Block.FlowingLava.id;
                                                }
                                                else
                                                {
                                                    blocks[blockIndex] = 0;
                                                    if (isGrassBlock && blocks[blockIndex - 1] == Block.Dirt.id)
                                                    {
                                                        blocks[blockIndex - 1] = (byte)Block.GrassBlock.id;
                                                    }
                                                }
                                            }
                                        }

                                        --blockIndex;
                                    }
                                }
                            }
                        }

                        if (var52)
                        {
                            break;
                        }
                    }
                }
            }
        }

    }
    protected override void CarveCaves(World world, int chunkX, int chunkZ, int centerChunkX, int centerChunkZ, byte[] blocks)
    {
        int var7 = rand.NextInt(rand.NextInt(rand.NextInt(40) + 1) + 1);
        if (rand.NextInt(15) != 0)
        {
            var7 = 0;
        }

        for (int offsetZ = 0; offsetZ < var7; ++offsetZ)
        {
            double var9 = chunkX * 16 + rand.NextInt(16);
            double var11 = rand.NextInt(rand.NextInt(120) + 8);
            double tunnelStep = chunkZ * 16 + rand.NextInt(16);
            int var15 = 1;
            if (rand.NextInt(4) == 0)
            {
                CarveCavesInChunk(centerChunkX, centerChunkZ, blocks, var9, var11, tunnelStep);
                var15 += rand.NextInt(4);
            }

            for (int var16 = 0; var16 < var15; ++var16)
            {
                float chunkCenterX = rand.NextFloat() * (float)Math.PI * 2.0F;
                float var18 = (rand.NextFloat() - 0.5F) * 2.0F / 8.0F;
                float chunkCenterZ = rand.NextFloat() * 2.0F + rand.NextFloat();
                CarveCaves(centerChunkX, centerChunkZ, blocks, var9, var11, tunnelStep, chunkCenterZ, chunkCenterX, var18, 0, 0, 1.0D);
            }
        }

    }
}
