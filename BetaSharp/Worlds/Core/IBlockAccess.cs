using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Generation.Biomes.Source;

namespace BetaSharp.Worlds.Core;

public interface IBlockAccess
{
    int getBlockId(int x, int y, int z);

    BlockEntity? getBlockEntity(int x, int y, int z);

    float getNaturalBrightness(int x, int y, int z, int blockLight);

    float getLuminance(int x, int y, int z);

    int getBlockMeta(int x, int y, int z);

    Material getMaterial(int x, int y, int z);

    bool isOpaque(int x, int y, int z);

    bool shouldSuffocate(int x, int y, int z);

    BiomeSource getBiomeSource();
}
