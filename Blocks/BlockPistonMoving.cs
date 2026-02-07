using betareborn.Entities;
using betareborn.Materials;
using betareborn.TileEntities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockPistonMoving : BlockContainer
    {
        public BlockPistonMoving(int var1) : base(var1, Material.PISTON)
        {
            setHardness(-1.0F);
        }

        protected override TileEntity getBlockEntity()
        {
            return null;
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            TileEntity var5 = var1.getBlockTileEntity(var2, var3, var4);
            if (var5 != null && var5 is TileEntityPiston)
            {
                ((TileEntityPiston)var5).finish();
            }
            else
            {
                base.onBlockRemoval(var1, var2, var3, var4);
            }

        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return false;
        }

        public override bool canPlaceBlockOnSide(World var1, int var2, int var3, int var4, int var5)
        {
            return false;
        }

        public override int getRenderType()
        {
            return -1;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (!var1.multiplayerWorld && var1.getBlockTileEntity(var2, var3, var4) == null)
            {
                var1.setBlockWithNotify(var2, var3, var4, 0);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return 0;
        }

        public override void dropStacks(World var1, int var2, int var3, int var4, int var5, float var6)
        {
            if (!var1.multiplayerWorld)
            {
                TileEntityPiston var7 = func_31034_c(var1, var2, var3, var4);
                if (var7 != null)
                {
                    Block.BLOCKS[var7.getPushedBlockId()].dropBlockAsItem(var1, var2, var3, var4, var7.getPushedBlockData());
                }
            }
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (!var1.multiplayerWorld && var1.getBlockTileEntity(var2, var3, var4) == null)
            {
            }

        }

        public static TileEntity func_31036_a(int var0, int var1, int var2, bool var3, bool var4)
        {
            return new TileEntityPiston(var0, var1, var2, var3, var4);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            TileEntityPiston var5 = func_31034_c(var1, var2, var3, var4);
            if (var5 == null)
            {
                return null;
            }
            else
            {
                float var6 = var5.getProgress(0.0F);
                if (var5.isExtending())
                {
                    var6 = 1.0F - var6;
                }

                return getPushedBlockCollisionShape(var1, var2, var3, var4, var5.getPushedBlockId(), var6, var5.getFacing());
            }
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            TileEntityPiston var5 = func_31034_c(var1, var2, var3, var4);
            if (var5 != null)
            {
                Block var6 = Block.BLOCKS[var5.getPushedBlockId()];
                if (var6 == null || var6 == this)
                {
                    return;
                }

                var6.updateBoundingBox(var1, var2, var3, var4);
                float var7 = var5.getProgress(0.0F);
                if (var5.isExtending())
                {
                    var7 = 1.0F - var7;
                }

                int var8 = var5.getFacing();
                minX = var6.minX - (double)((float)PistonBlockTextures.field_31056_b[var8] * var7);
                minY = var6.minY - (double)((float)PistonBlockTextures.field_31059_c[var8] * var7);
                minZ = var6.minZ - (double)((float)PistonBlockTextures.field_31058_d[var8] * var7);
                maxX = var6.maxX - (double)((float)PistonBlockTextures.field_31056_b[var8] * var7);
                maxY = var6.maxY - (double)((float)PistonBlockTextures.field_31059_c[var8] * var7);
                maxZ = var6.maxZ - (double)((float)PistonBlockTextures.field_31058_d[var8] * var7);
            }

        }

        public Box getPushedBlockCollisionShape(World var1, int var2, int var3, int var4, int var5, float var6, int var7)
        {
            if (var5 != 0 && var5 != id)
            {
                Box var8 = Block.BLOCKS[var5].getCollisionShape(var1, var2, var3, var4);
                if (var8 == null)
                {
                    return null;
                }
                else
                {
                    var8.minX -= (double)((float)PistonBlockTextures.field_31056_b[var7] * var6);
                    var8.maxX -= (double)((float)PistonBlockTextures.field_31056_b[var7] * var6);
                    var8.minY -= (double)((float)PistonBlockTextures.field_31059_c[var7] * var6);
                    var8.maxY -= (double)((float)PistonBlockTextures.field_31059_c[var7] * var6);
                    var8.minZ -= (double)((float)PistonBlockTextures.field_31058_d[var7] * var6);
                    var8.maxZ -= (double)((float)PistonBlockTextures.field_31058_d[var7] * var6);
                    return var8;
                }
            }
            else
            {
                return null;
            }
        }

        private TileEntityPiston func_31034_c(BlockView var1, int var2, int var3, int var4)
        {
            TileEntity var5 = var1.getBlockTileEntity(var2, var3, var4);
            return var5 != null && var5 is TileEntityPiston ? (TileEntityPiston)var5 : null;
        }
    }

}