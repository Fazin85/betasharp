using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Gen.Features;

public class OreFeature : Feature
{

    private readonly int _minableBlockId;
    private readonly int _numberOfBlocks;

    public OreFeature(int minableBlockId, int numberOfBlocks)
    {
        _minableBlockId = minableBlockId;
        _numberOfBlocks = numberOfBlocks;
    }

    public override bool Generate(World world, JavaRandom rand, int x, int y, int z)
    {
        float angle = rand.NextFloat() * (float)Math.PI;
        double spread = _numberOfBlocks / 8.0;

        double startX = x + 8 + MathHelper.sin(angle) * spread;
        double endX = x + 8 - MathHelper.sin(angle) * spread;
        double startZ = z + 8 + MathHelper.cos(angle) * spread;
        double endZ = z + 8 - MathHelper.cos(angle) * spread;

        double startY = y + rand.NextInt(3) + 2;
        double endY = y + rand.NextInt(3) + 2;

        for (int i = 0; i <= _numberOfBlocks; ++i)
        {
            double centerX = startX + (endX - startX) * i / _numberOfBlocks;
            double centerY = startY + (endY - startY) * i / _numberOfBlocks;
            double centerZ = startZ + (endZ - startZ) * i / _numberOfBlocks;

            double sizeMultiplier = rand.NextDouble() * _numberOfBlocks / 16.0D;
            double radiusH = (MathHelper.sin(i * (float)Math.PI / _numberOfBlocks) + 1.0F) * sizeMultiplier + 1.0D;
            double radiusV = (MathHelper.sin(i * (float)Math.PI / _numberOfBlocks) + 1.0F) * sizeMultiplier + 1.0D;

            int minX = MathHelper.floor_double(centerX - radiusH / 2.0D);
            int minY = MathHelper.floor_double(centerY - radiusV / 2.0D);
            int minZ = MathHelper.floor_double(centerZ - radiusH / 2.0D);
            int maxX = MathHelper.floor_double(centerX + radiusH / 2.0D);
            int maxY = MathHelper.floor_double(centerY + radiusV / 2.0D);
            int maxZ = MathHelper.floor_double(centerZ + radiusH / 2.0D);

            for (int blockX = minX; blockX <= maxX; ++blockX)
            {
                double dx = (blockX + 0.5 - centerX) / (radiusH / 2.0);
                if (dx * dx >= 1.0) continue;

                for (int blockY = minY; blockY <= maxY; ++blockY)
                {
                    double dy = (blockY + 0.5 - centerY) / (radiusV / 2.0);
                    if (dx * dx + dy * dy >= 1.0) continue;

                    for (int blockZ = minZ; blockZ <= maxZ; ++blockZ)
                    {
                        double dz = (blockZ + 0.5 - centerZ) / (radiusH / 2.0);

                        if (dx * dx + dy * dy + dz * dz < 1.0 && world.getBlockId(blockX, blockY, blockZ) == Block.Stone.id)
                        {
                            world.SetBlockWithoutNotifyingNeighbors(blockX, blockY, blockZ, _minableBlockId);
                        }
                    }
                }
            }
        }

        return true;
    }
}
