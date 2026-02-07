using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockTNT : Block
    {
        public BlockTNT(int var1, int var2) : base(var1, var2, Material.TNT)
        {
        }

        public override int getTexture(int var1)
        {
            return var1 == 0 ? textureId + 2 : (var1 == 1 ? textureId + 1 : textureId);
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            base.onBlockAdded(var1, var2, var3, var4);
            if (var1.isBlockIndirectlyGettingPowered(var2, var3, var4))
            {
                onBlockDestroyedByPlayer(var1, var2, var3, var4, 1);
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (var5 > 0 && Block.BLOCKS[var5].canProvidePower() && var1.isBlockIndirectlyGettingPowered(var2, var3, var4))
            {
                onBlockDestroyedByPlayer(var1, var2, var3, var4, 1);
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 0;
        }

        public override void onBlockDestroyedByExplosion(World var1, int var2, int var3, int var4)
        {
            EntityTNTPrimed var5 = new EntityTNTPrimed(var1, (double)((float)var2 + 0.5F), (double)((float)var3 + 0.5F), (double)((float)var4 + 0.5F));
            var5.fuse = var1.random.nextInt(var5.fuse / 4) + var5.fuse / 8;
            var1.spawnEntity(var5);
        }

        public override void onBlockDestroyedByPlayer(World var1, int var2, int var3, int var4, int var5)
        {
            if (!var1.multiplayerWorld)
            {
                if ((var5 & 1) == 0)
                {
                    dropBlockAsItem_do(var1, var2, var3, var4, new ItemStack(Block.TNT.id, 1, 0));
                }
                else
                {
                    EntityTNTPrimed var6 = new EntityTNTPrimed(var1, (double)((float)var2 + 0.5F), (double)((float)var3 + 0.5F), (double)((float)var4 + 0.5F));
                    var1.spawnEntity(var6);
                    var1.playSoundAtEntity(var6, "random.fuse", 1.0F, 1.0F);
                }

            }
        }

        public override void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (var5.getCurrentEquippedItem() != null && var5.getCurrentEquippedItem().itemID == Item.flintAndSteel.id)
            {
                var1.setBlockMetadata(var2, var3, var4, 1);
            }

            base.onBlockClicked(var1, var2, var3, var4, var5);
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            return base.onUse(var1, var2, var3, var4, var5);
        }
    }

}