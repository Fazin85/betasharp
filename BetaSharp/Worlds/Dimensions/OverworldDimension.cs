using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Gen.Chunks;
using BetaSharp.Worlds.Gen.Flat;
using BetaSharp.Worlds.Generation.Generators.Chunks;

namespace BetaSharp.Worlds.Dimensions;

internal class OverworldDimension : Dimension
{
    public override IChunkSource CreateChunkGenerator()
    {
        WorldType terrainType = World.getProperties().TerrainType;

        if (terrainType == WorldType.Flat)
        {
            return new FlatIChunkGenerator(World);
        }

        if (terrainType == WorldType.Sky)
        {
            return new SkyIChunkGenerator(World, World.getSeed());
        }

        return base.CreateChunkGenerator();
    }

    public override bool IsValidSpawnPoint(int x, int z)
    {
        if (World.getProperties().TerrainType == WorldType.Flat)
        {
            return true;
        }

        return base.IsValidSpawnPoint(x, z);
    }
}
