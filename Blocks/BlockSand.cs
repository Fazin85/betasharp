using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSand : Block
    {
        public static bool fallInstantly = false;

        public BlockSand(int id, int textureId) : base(id, textureId, Material.SAND)
        {
        }

        public override void OnPlaced(World world, int x, int y, int z)
        {
            world.scheduleBlockUpdate(x, y, z, id, GetTickRate());
        }

        public override void NeighborUpdate(World world, int x, int y, int z, int id)
        {
            world.scheduleBlockUpdate(x, y, z, base.id, GetTickRate());
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            _ProcessFall(world, x, y, z);
        }

        private void _ProcessFall(World world, int x, int y, int z)
        {
            if (!(CanFallThrough(world, x, y - 1, z) && y >= 0)) return;

            const sbyte checkRadius = 32;
            if (!fallInstantly && world.isRegionLoaded(x - checkRadius, y - checkRadius, z - checkRadius, x + checkRadius, y + checkRadius, z + checkRadius))
            {
                var fallingSand = new EntityFallingSand(world, x + 0.5f, y + 0.5f, z + 0.5f, id);
                world.spawnEntity(fallingSand);
            }
            else
            {
                world.setBlock(x, y, z, 0);

                while (CanFallThrough(world, x, y - 1, z) && y > 0)
                {
                    --y;
                }

                if (y > 0)
                {
                    world.setBlock(x, y, z, id);
                }
            }
        }

        public override int GetTickRate()
        {
            return 3;
        }

        public static bool CanFallThrough(World world, int x, int y, int z)
        {
            int blockId = world.getBlockId(x, y, z);
            if (blockId == 0 || blockId == FIRE.id) 
                return true;

            var material = BLOCKS[blockId].material;
            return material == Material.WATER || material == Material.LAVA;
        }
    }

}