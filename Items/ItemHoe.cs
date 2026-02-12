using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemHoe : Item
    {

        public ItemHoe(int id, EnumToolMaterial enumToolMaterial) : base(id)
        {
            maxCount = 1;
            setMaxDamage(enumToolMaterial.getMaxUses());
        }

        public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, World world, int x, int y, int z, int meta)
        {
            int targetBlockId = world.GetBlockId(x, y, z);
            int blockAbove = world.GetBlockId(x, y + 1, z);
            if ((meta == 0 || blockAbove != 0 || targetBlockId != Block.GRASS_BLOCK.id) && targetBlockId != Block.DIRT.id)
            {
                return false;
            }
            else
            {
                Block block = Block.FARMLAND;
                world.playSound((double)((float)x + 0.5F), (double)((float)y + 0.5F), (double)((float)z + 0.5F), block.soundGroup.func_1145_d(), (block.soundGroup.getVolume() + 1.0F) / 2.0F, block.soundGroup.getPitch() * 0.8F);
                if (world.isRemote)
                {
                    return true;
                }
                else
                {
                    world.setBlock(x, y, z, block.id);
                    itemStack.damageItem(1, entityPlayer);
                    return true;
                }
            }
        }

        public override bool isHandheld()
        {
            return true;
        }
    }

}