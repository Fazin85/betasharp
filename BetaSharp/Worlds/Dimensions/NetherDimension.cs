using BetaSharp.Blocks;
using BetaSharp.Worlds.Biomes;
using BetaSharp.Worlds.Biomes.Source;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Gen.Chunks;
using Silk.NET.Maths;

namespace BetaSharp.Worlds.Dimensions;

public class NetherDimension : Dimension
{
    public override void initBiomeSource()
    {
        biomeSource = new FixedBiomeSource(Biome.Hell, 1.0D, 0.0D);
        isNether = true;
        evaporatesWater = true;
        hasCeiling = true;
        id = -1;
    }

    public override Vector3D<double> getFogColor(float var1, float var2)
    {
        return new((double)0.2F, (double)0.03F, (double)0.03F);
    }

    protected override void initBrightnessTable()
    {
        float var1 = 0.1F;

        for (int var2 = 0; var2 <= 15; ++var2)
        {
            float var3 = 1.0F - var2 / 15.0F;
            lightLevelToLuminance[var2] = (1.0F - var3) / (var3 * 3.0F + 1.0F) * (1.0F - var1) + var1;
        }

    }

    public override ChunkSource createChunkGenerator()
    {
        return new NetherChunkGenerator(world, world.getSeed());
    }

    public override bool isValidSpawnPoint(int var1, int var2)
    {
        int var3 = world.getSpawnBlockId(var1, var2);
        return var3 == Block.Bedrock.id ? false : var3 == 0 ? false : Block.BlocksOpaque[var3];
    }

    public override float getTimeOfDay(long var1, float var3)
    {
        return 0.5F;
    }

    public override bool hasWorldSpawn()
    {
        return false;
    }
}