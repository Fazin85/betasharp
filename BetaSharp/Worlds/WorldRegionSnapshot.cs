using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Biomes.Source;
using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds;

public class WorldRegionSnapshot : BlockView, IDisposable
{
    private readonly int chunkX;
    private readonly int chunkZ;
    private readonly ChunkSnapshot[][] chunkArray;
    private readonly float[] lightTable;
    private readonly int skylightSubtracted;
    private readonly BiomeSource biomeSource;
    private bool isLit = false;

    public WorldRegionSnapshot(World var1, int var2, int var3, int var4, int var5, int var6, int var7)
    {
        //TODO: OPTIMIZE THIS
        biomeSource = new(var1);

        chunkX = var2 >> 4;
        chunkZ = var4 >> 4;
        int var8 = var5 >> 4;
        int var9 = var7 >> 4;
        chunkArray = new ChunkSnapshot[var8 - chunkX + 1][];
        for (int i = 0; i < chunkArray.Length; i++)
        {
            chunkArray[i] = new ChunkSnapshot[var9 - chunkZ + 1];
        }

        for (int var10 = chunkX; var10 <= var8; ++var10)
        {
            for (int var11 = chunkZ; var11 <= var9; ++var11)
            {
                chunkArray[var10 - chunkX][var11 - chunkZ] = new(var1.getChunk(var10, var11));
            }
        }

        lightTable = new float[var1.dimension.lightLevelToLuminance.Length];
        Buffer.BlockCopy(var1.dimension.lightLevelToLuminance, 0, lightTable, 0, sizeof(float) * lightTable.Length);
        skylightSubtracted = var1.ambientDarkness;
    }

    public int getBlockId(int var1, int var2, int var3)
    {
        if (var2 < 0)
        {
            return 0;
        }
        else if (var2 >= 128)
        {
            return 0;
        }
        else
        {
            int var4 = (var1 >> 4) - chunkX;
            int var5 = (var3 >> 4) - chunkZ;
            if (var4 >= 0 && var4 < chunkArray.Length && var5 >= 0 && var5 < chunkArray[var4].Length)
            {
                ChunkSnapshot var6 = chunkArray[var4][var5];
                return var6 == null ? 0 : var6.getBlockID(var1 & 15, var2, var3 & 15);
            }
            else
            {
                return 0;
            }
        }
    }

    public Material getMaterial(int var1, int var2, int var3)
    {
        int var4 = getBlockId(var1, var2, var3);
        return var4 == 0 ? Material.Air : Block.BLOCKS[var4].material;
    }

    public int getBlockMeta(int var1, int var2, int var3)
    {
        if (var2 < 0)
        {
            return 0;
        }
        else if (var2 >= 128)
        {
            return 0;
        }
        else
        {
            int var4 = (var1 >> 4) - chunkX;
            int var5 = (var3 >> 4) - chunkZ;
            return chunkArray[var4][var5].getBlockMetadata(var1 & 15, var2, var3 & 15);
        }
    }

    public BlockEntity getBlockEntity(int var1, int var2, int var3)
    {
        throw new NotImplementedException();
    }

    public float getNaturalBrightness(int var1, int var2, int var3, int var4)
    {
        int var5 = getLightValue(var1, var2, var3);
        if (var5 < var4)
        {
            var5 = var4;
        }

        return lightTable[var5];
    }

    public float getLuminance(int var1, int var2, int var3)
    {
        return lightTable[getLightValue(var1, var2, var3)];
    }

    public int getLightValue(int var1, int var2, int var3)
    {
        return getLightValueExt(var1, var2, var3, true);
    }

    public int getLightValueExt(int var1, int var2, int var3, bool var4)
    {
        if (var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000)
        {
            int var5;
            int var6;
            if (var4)
            {
                var5 = getBlockId(var1, var2, var3);
                if (var5 == Block.SLAB.id || var5 == Block.FARMLAND.id || var5 == Block.WOODEN_STAIRS.id || var5 == Block.COBBLESTONE_STAIRS.id)
                {
                    var6 = getLightValueExt(var1, var2 + 1, var3, false);
                    int var7 = getLightValueExt(var1 + 1, var2, var3, false);
                    int var8 = getLightValueExt(var1 - 1, var2, var3, false);
                    int var9 = getLightValueExt(var1, var2, var3 + 1, false);
                    int var10 = getLightValueExt(var1, var2, var3 - 1, false);
                    if (var7 > var6)
                    {
                        var6 = var7;
                    }

                    if (var8 > var6)
                    {
                        var6 = var8;
                    }

                    if (var9 > var6)
                    {
                        var6 = var9;
                    }

                    if (var10 > var6)
                    {
                        var6 = var10;
                    }

                    return var6;
                }
            }

            if (var2 < 0)
            {
                return 0;
            }
            else if (var2 >= 128)
            {
                var5 = 15 - skylightSubtracted;
                if (var5 < 0)
                {
                    var5 = 0;
                }

                return var5;
            }
            else
            {
                var5 = (var1 >> 4) - chunkX;
                var6 = (var3 >> 4) - chunkZ;

                ChunkSnapshot chunk = chunkArray[var5][var6];
                int lightValue = chunk.getBlockLightValue(var1 & 15, var2, var3 & 15, skylightSubtracted);

                if (chunk.getIsLit())
                {
                    isLit = true;
                }

                return lightValue;
            }
        }
        else
        {
            return 15;
        }
    }

    public BiomeSource getBiomeSource()
    {
        return biomeSource;
    }

    public bool shouldSuffocate(int var1, int var2, int var3)
    {
        Block var4 = Block.BLOCKS[getBlockId(var1, var2, var3)];
        return var4 == null ? false : var4.material.BlocksMovement && var4.isFullCube();
    }

    public bool isOpaque(int var1, int var2, int var3)
    {
        Block var4 = Block.BLOCKS[getBlockId(var1, var2, var3)];
        return var4 == null ? false : var4.isOpaque();
    }

    public bool getIsLit()
    {
        return isLit;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach (var column in chunkArray)
        {
            if (column == null) continue;

            foreach (var snapshot in column)
            {
                snapshot?.Dispose();
            }
        }
    }
}