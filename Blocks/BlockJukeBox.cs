using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.TileEntities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockJukeBox : BlockContainer
    {

        public BlockJukeBox(int var1, int var2) : base(var1, var2, Material.WOOD)
        {
        }

        public override int getTexture(int var1)
        {
            return textureId + (var1 == 1 ? 1 : 0);
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (var1.getBlockMeta(var2, var3, var4) == 0)
            {
                return false;
            }
            else
            {
                func_28038_b_(var1, var2, var3, var4);
                return true;
            }
        }

        public void ejectRecord(World var1, int var2, int var3, int var4, int var5)
        {
            if (!var1.multiplayerWorld)
            {
                TileEntityRecordPlayer var6 = (TileEntityRecordPlayer)var1.getBlockTileEntity(var2, var3, var4);
                var6.recordId = var5;
                var6.markDirty();
                var1.setBlockMeta(var2, var3, var4, 1);
            }
        }

        public void func_28038_b_(World var1, int var2, int var3, int var4)
        {
            if (!var1.multiplayerWorld)
            {
                TileEntityRecordPlayer var5 = (TileEntityRecordPlayer)var1.getBlockTileEntity(var2, var3, var4);
                int var6 = var5.recordId;
                if (var6 != 0)
                {
                    var1.func_28106_e(1005, var2, var3, var4, 0);
                    var1.playRecord((String)null, var2, var3, var4);
                    var5.recordId = 0;
                    var5.markDirty();
                    var1.setBlockMeta(var2, var3, var4, 0);
                    float var8 = 0.7F;
                    double var9 = (double)(var1.random.nextFloat() * var8) + (double)(1.0F - var8) * 0.5D;
                    double var11 = (double)(var1.random.nextFloat() * var8) + (double)(1.0F - var8) * 0.2D + 0.6D;
                    double var13 = (double)(var1.random.nextFloat() * var8) + (double)(1.0F - var8) * 0.5D;
                    EntityItem var15 = new EntityItem(var1, (double)var2 + var9, (double)var3 + var11, (double)var4 + var13, new ItemStack(var6, 1, 0));
                    var15.delayBeforeCanPickup = 10;
                    var1.spawnEntity(var15);
                }
            }
        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            func_28038_b_(var1, var2, var3, var4);
            base.onBlockRemoval(var1, var2, var3, var4);
        }

        public override void dropStacks(World var1, int var2, int var3, int var4, int var5, float var6)
        {
            if (!var1.multiplayerWorld)
            {
                base.dropStacks(var1, var2, var3, var4, var5, var6);
            }
        }

        protected override TileEntity getBlockEntity()
        {
            return new TileEntityRecordPlayer();
        }
    }

}