using betareborn.Blocks.Entities;
using betareborn.Blocks.Materials;
using betareborn.Worlds.Biomes.Source;

namespace betareborn.Worlds
{
    public interface BlockView
    {
        int GetBlockId(int x, int y, int z);

        BlockEntity GetBlockEntity(int x, int y, int z);

        float GetNaturalBrightness(int x, int y, int z, int blockLight);

        float GetLuminance(int x, int y, int z);

        int GetBlockMeta(int x, int y, int z);

        Material GetMaterial(int x, int y, int z);

        bool IsOpaque(int x, int y, int z);

        bool ShouldSuffocate(int x, int y, int z);

        BiomeSource BiomeSource { get; }
    }

}