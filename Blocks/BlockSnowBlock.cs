using betareborn.Blocks.Materials;
using betareborn.Items;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSnowBlock : Block
    {

        public BlockSnowBlock(int id, int textureId) : base(id, textureId, Material.SNOW_BLOCK)
        {
            setTickRandomly(true);
        }

        public override int getDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Item.SNOWBALL.id;
        }

        public override int getDroppedItemCount(java.util.Random random)
        {
            return 4;
        }

        public override void onTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (world.getBrightness(LightType.Block, x, y, z) > 11)
            {
                dropStacks(world, x, y, z, world.GetBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
            }

        }
    }

}