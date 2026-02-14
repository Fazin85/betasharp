using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Colors;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockGrass : Block
{
    public BlockGrass(int id) : base(id, Material.SolidOrganic)
    {
        textureId = 3;
        setTickRandomly(true);
    }

    public override int getTextureId(BlockView blockView, int x, int y, int z, int side)
    {
        if (side == 1)
        {
            return 0;
        }
        else if (side == 0)
        {
            return 2;
        }
        else
        {
            Material materialAbove = blockView.getMaterial(x, y + 1, z);
            return materialAbove != Material.SnowLayer && materialAbove != Material.SnowBlock ? 3 : 68;
        }
    }

    public override int getColorMultiplier(BlockView blockView, int x, int y, int z)
    {
        blockView.getBiomeSource().getBiomesInArea(x, z, 1, 1);
        double temperature = blockView.getBiomeSource().temperatureMap[0];
        double downfall = blockView.getBiomeSource().downfallMap[0];
        return GrassColors.getColor(temperature, downfall);
    }

    public override void onTick(World world, int x, int y, int z, java.util.Random random)
    {
        if (!world.isRemote)
        {
            if (world.getLightLevel(x, y + 1, z) < 4 && Block.BLOCK_LIGHT_OPACITY[world.getBlockId(x, y + 1, z)] > 2)
            {
                if (random.nextInt(4) != 0)
                {
                    return;
                }

                world.setBlock(x, y, z, Block.DIRT.id);
            }
            else if (world.getLightLevel(x, y + 1, z) >= 9)
            {
                int spreadX = x + random.nextInt(3) - 1;
                int spreadY = y + random.nextInt(5) - 3;
                int spreadZ = z + random.nextInt(3) - 1;
                int blockAboveId = world.getBlockId(spreadX, spreadY + 1, spreadZ);
                if (world.getBlockId(spreadX, spreadY, spreadZ) == Block.DIRT.id && world.getLightLevel(spreadX, spreadY + 1, spreadZ) >= 4 && Block.BLOCK_LIGHT_OPACITY[blockAboveId] <= 2)
                {
                    world.setBlock(spreadX, spreadY, spreadZ, Block.GRASS_BLOCK.id);
                }
            }

        }
    }

    public override int getDroppedItemId(int blocKMeta, java.util.Random random)
    {
        return Block.DIRT.getDroppedItemId(0, random);
    }
}