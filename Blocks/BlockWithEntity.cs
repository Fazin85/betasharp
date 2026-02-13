using betareborn.Worlds;
using betareborn.Blocks.Materials;
using betareborn.Blocks.Entities;

namespace betareborn.Blocks
{
    public abstract class BlockWithEntity : Block
    {

        protected BlockWithEntity(int id, Material material) : base(id, material)
        {
            BLOCKS_WITH_ENTITY[id] = true;
        }

        protected BlockWithEntity(int id, int textureId, Material material) : base(id, textureId, material)
        {
            BLOCKS_WITH_ENTITY[id] = true;
        }

        public override void OnPlaced(World world, int x, int y, int z)
        {
            base.OnPlaced(world, x, y, z);
            world.setBlockEntity(x, y, z, getBlockEntity());
        }

        public override void onBreak(World world, int x, int y, int z)
        {
            base.onBreak(world, x, y, z);
            world.removeBlockEntity(x, y, z);
        }

        protected abstract BlockEntity getBlockEntity();
    }

}