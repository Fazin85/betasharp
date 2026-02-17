using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class GrassPatchFeature : Feature
{

    private readonly int _tallGrassBlockId;
    private readonly int _tallGrassBlockMeta;

    public GrassPatchFeature(int tallGrassBlockId, int tallGrassBlockMeta)
    {
        _tallGrassBlockId = tallGrassBlockId;
        _tallGrassBlockMeta = tallGrassBlockMeta;
    }

    public override bool Generate(World world, java.util.Random rand, int x, int y, int z)
    {
        while (true)
        {
            int blockId = world.getBlockId(x, y, z);
            if (blockId != 0 && blockId != Block.Leaves.id || y <= 0)
            {
                for (int i = 0; i < 128; ++i)
                {
                    int genX = x + rand.nextInt(8) - rand.nextInt(8);
                    int genY = y + rand.nextInt(4) - rand.nextInt(4);
                    int genZ = z + rand.nextInt(8) - rand.nextInt(8);
                    if (world.isAir(genX, genY, genZ) && ((BlockPlant)Block.Blocks[_tallGrassBlockId]).canGrow(world, genX, genY, genZ))
                    {
                        world.SetBlockWithoutNotifyingNeighbors(genX, genY, genZ, _tallGrassBlockId, _tallGrassBlockMeta);
                    }
                }

                return true;
            }

            --y;
        }
    }
}
