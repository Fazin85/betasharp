using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Colors;
using javax.swing.text.html;
using BlockView = BetaSharp.Worlds.BlockView;

namespace BetaSharp.Blocks;

public class BlockGrass : Block
{
    public BlockGrass(int id) : base(id, Material.SolidOrganic)
    {
        textureId = "grassgrass_top";
        setTickRandomly(true);
    }

    public override string getTextureId(BlockView blockView, int x, int y, int z, string side)
    {
        if (side == "top")
        {
            return "grass_top";
        }
        else if (side == "bottom")
        {
            return "dirt";
        }
        else // for sides
        {
            return "grass_side";
        }
    }

    public override string getTexture(string side, int meta)
    {
        if (side == "top")
        {
            return "grass_top";
        }
        else if (side == "bottom")
        {
            return "dirt";
        }
        else // for sides
        {
            return "grass";
        }
    }

    /*
    public override int getColorMultiplier(BlockView blockView, int x, int y, int z)
    {
        blockView.getBiomeSource().GetBiomesInArea(x, z, 1, 1);
        double temperature = blockView.getBiomeSource().TemperatureMap[0];
        double downfall = blockView.getBiomeSource().DownfallMap[0]; bullshit for now, i'll change to be brighter and have the original Classic texture
        return GrassColors.getColor(temperature, downfall);
    }*/ 

    public override void onTick(World world, int x, int y, int z, JavaRandom random)
    {
        if (!world.isRemote)
        {
            if (world.getLightLevel(x, y + 1, z) < 4 && Block.BlockLightOpacity[world.getBlockId(x, y + 1, z)] > 2)
            {
                if (random.NextInt(4) != 0)
                {
                    return;
                }

                world.setBlock(x, y, z, Block.Dirt.id);
            }
            else if (world.getLightLevel(x, y + 1, z) >= 9)
            {
                int spreadX = x + random.NextInt(3) - 1;
                int spreadY = y + random.NextInt(5) - 3;
                int spreadZ = z + random.NextInt(3) - 1;
                int blockAboveId = world.getBlockId(spreadX, spreadY + 1, spreadZ);
                if (world.getBlockId(spreadX, spreadY, spreadZ) == Block.Dirt.id && world.getLightLevel(spreadX, spreadY + 1, spreadZ) >= 4 && Block.BlockLightOpacity[blockAboveId] <= 2)
                {
                    world.setBlock(spreadX, spreadY, spreadZ, Block.GrassBlock.id);
                }
            }
        }
    }

    public override int getDroppedItemId(int blocKMeta, JavaRandom random)
    {
        return Block.Dirt.getDroppedItemId(0, random);
    }
}
