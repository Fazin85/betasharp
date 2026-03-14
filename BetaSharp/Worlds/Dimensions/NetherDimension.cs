using BetaSharp.Blocks;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Generation.Biomes;
using BetaSharp.Worlds.Generation.Biomes.Source;
using BetaSharp.Worlds.Generation.Generators.Chunks;
using Silk.NET.Maths;

namespace BetaSharp.Worlds.Dimensions;

internal class NetherDimension : Dimension
{
    public override bool HasWorldSpawn => false;

    public override void InitBiomeSource()
    {
        BiomeSource = new FixedBiomeSource(World, Biome.Hell, 1.0D, 0.0D);
        IsNether = true;
        EvaporatesWater = true;
        HasCeiling = true;
        Id = -1;
    }

    public override Vector3D<double> GetFogColor(float celestialAngle, float partialTicks) => new(0.2, 0.03, 0.03);

    protected override void InitBrightnessTable()
    {
        float offset = 0.1F;

        for (int i = 0; i <= 15; ++i)
        {
            float factor = 1.0F - i / 15.0F;
            LightLevelToLuminance[i] = (1.0F - factor) / (factor * 3.0F + 1.0F) * (1.0F - offset) + offset;
        }
    }

    public override IChunkSource CreateChunkGenerator() => new NetherIChunkGenerator(World, World.Seed);

    public override bool IsValidSpawnPoint(int x, int z)
    {
        int y = World.Reader.GetTopY(x, z);
        int topBlockId = World.Reader.GetBlockId(x, y, z);
        return topBlockId != 0 && Block.Blocks[topBlockId] != null && Block.Blocks[topBlockId].material.BlocksMovement;
    }

    public override float GetTimeOfDay(long time, float tickDelta) => 0.5F;
}
