using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Gen.Features;

public class ClayOreFeature : Feature
{

    private readonly int _clayBlockId = Block.Clay.id;
    private readonly int _numberOfBlocks;

    public ClayOreFeature(int numberOfBlocks)
    {
        _numberOfBlocks = numberOfBlocks;
    }

    public override bool Generate(World world, JavaRandom rand, int x, int y, int z)
    {
        if (world.getMaterial(x, y, z) != Material.Water)
        {
            return false;
        }

        float angle = rand.NextFloat() * (float)Math.PI;
        double spread = _numberOfBlocks / 8.0;

        double startX = x + 8 + MathHelper.sin(angle) * spread;
        double endX = x + 8 - MathHelper.sin(angle) * spread;
        double startZ = z + 8 + MathHelper.cos(angle) * spread;
        double enZ = z + 8 - MathHelper.cos(angle) * spread;

        double startY = y + rand.NextInt(3) + 2;
        double endY = y + rand.NextInt(3) + 2;

        for (int i = 0; i <= _numberOfBlocks; ++i)
        {
            float lerp = i * _numberOfBlocks;
            double centerX = startX + (endX - startX) * lerp;
            double centerY = startY + (endY - startY) * lerp;
            double centerZ = startZ + (enZ - startZ) * lerp;

            double sizeMultiplier = rand.NextDouble() * _numberOfBlocks / 16.0D;
            double radiusH = (double)(MathHelper.sin(i * (float)Math.PI / _numberOfBlocks) + 1.0F) * sizeMultiplier + 1.0D;
            double radiusV = (double)(MathHelper.sin(i * (float)Math.PI / _numberOfBlocks) + 1.0F) * sizeMultiplier + 1.0D;

            int minX = MathHelper.floor_double(centerX - radiusH / 2.0D);
            int minY = MathHelper.floor_double(centerY - radiusV / 2.0D);
            int minZ = MathHelper.floor_double(centerZ - radiusH / 2.0D);

            int maxX = MathHelper.floor_double(centerX + radiusH / 2.0D);
            int maxY = MathHelper.floor_double(centerY + radiusV / 2.0D);
            int maxZ = MathHelper.floor_double(centerZ + radiusH / 2.0D);

            for (int blockX = minX; blockX <= maxX; ++blockX)
            {
                for (int blockY = minY; blockY <= maxY; ++blockY)
                {
                    for (int blockZ = minZ; blockZ <= maxZ; ++blockZ)
                    {
                        double dx = (blockX + 0.5D - centerX) / (radiusH / 2.0D);
                        double dy = (blockY + 0.5D - centerY) / (radiusV / 2.0D);
                        double dz = (blockZ + 0.5D - centerZ) / (radiusH / 2.0D);
                        if (dx * dx + dy * dy + dz * dz < 1.0D)
                        {
                            int var47 = world.getBlockId(blockX, blockY, blockZ);
                            if (var47 == Block.Sand.id)
                            {
                                world.SetBlockWithoutNotifyingNeighbors(blockX, blockY, blockZ, _clayBlockId);
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
}
