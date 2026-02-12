using betareborn.Blocks.Entities;
using betareborn.Blocks.Materials;
using betareborn.Worlds.Biomes.Source;

namespace betareborn.Worlds
{
    public interface BlockView
    {
        int getBlockId(int x, int y, int z);

        BlockEntity getBlockEntity(int x, int y, int z);

        float getNaturalBrightness(int x, int y, int z, int blockLight);

        float getLuminance(int x, int y, int z);

        int getBlockMeta(int x, int y, int z);

        Material getMaterial(int x, int y, int z);

        bool isOpaque(int x, int y, int z);

        bool shouldSuffocate(int x, int y, int z);

        BiomeSource getBiomeSource();
    }

}