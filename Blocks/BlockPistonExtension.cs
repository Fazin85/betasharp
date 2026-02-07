using betareborn.Materials;
using betareborn.Worlds;
using java.util;

namespace betareborn.Blocks
{
    public class BlockPistonExtension : Block
    {
        private int field_31053_a = -1;

        public BlockPistonExtension(int var1, int var2) : base(var1, var2, Material.PISTON)
        {
            setSoundGroup(soundStoneFootstep);
            setHardness(0.5F);
        }

        public void func_31052_a_(int var1)
        {
            field_31053_a = var1;
        }

        public void func_31051_a()
        {
            field_31053_a = -1;
        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            base.onBlockRemoval(var1, var2, var3, var4);
            int var5 = var1.getBlockMeta(var2, var3, var4);
            int var6 = PistonBlockTextures.field_31057_a[func_31050_c(var5)];
            var2 += PistonBlockTextures.field_31056_b[var6];
            var3 += PistonBlockTextures.field_31059_c[var6];
            var4 += PistonBlockTextures.field_31058_d[var6];
            int var7 = var1.getBlockId(var2, var3, var4);
            if (var7 == Block.PISTON.id || var7 == Block.STICKY_PISTON.id)
            {
                var5 = var1.getBlockMeta(var2, var3, var4);
                if (BlockPistonBase.isPowered(var5))
                {
                    Block.BLOCKS[var7].dropBlockAsItem(var1, var2, var3, var4, var5);
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
            }

        }

        public override int getTexture(int var1, int var2)
        {
            int var3 = func_31050_c(var2);
            return var1 == var3 ? (field_31053_a >= 0 ? field_31053_a : ((var2 & 8) != 0 ? textureId - 1 : textureId)) : (var1 == PistonBlockTextures.field_31057_a[var3] ? 107 : 108);
        }

        public override int getRenderType()
        {
            return 17;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return false;
        }

        public override bool canPlaceBlockOnSide(World var1, int var2, int var3, int var4, int var5)
        {
            return false;
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 0;
        }

        public override void addIntersectingBoundingBox(World var1, int var2, int var3, int var4, Box var5, List<Box> var6)
        {
            int var7 = var1.getBlockMeta(var2, var3, var4);
            switch (func_31050_c(var7))
            {
                case 0:
                    setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    setBoundingBox(6.0F / 16.0F, 0.25F, 6.0F / 16.0F, 10.0F / 16.0F, 1.0F, 10.0F / 16.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    break;
                case 1:
                    setBoundingBox(0.0F, 12.0F / 16.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    setBoundingBox(6.0F / 16.0F, 0.0F, 6.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F, 10.0F / 16.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    break;
                case 2:
                    setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    setBoundingBox(0.25F, 6.0F / 16.0F, 0.25F, 12.0F / 16.0F, 10.0F / 16.0F, 1.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    break;
                case 3:
                    setBoundingBox(0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F, 1.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    setBoundingBox(0.25F, 6.0F / 16.0F, 0.0F, 12.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    break;
                case 4:
                    setBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    setBoundingBox(6.0F / 16.0F, 0.25F, 0.25F, 10.0F / 16.0F, 12.0F / 16.0F, 1.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    break;
                case 5:
                    setBoundingBox(12.0F / 16.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    setBoundingBox(0.0F, 6.0F / 16.0F, 0.25F, 12.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F);
                    base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
                    break;
            }

            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            switch (func_31050_c(var5))
            {
                case 0:
                    setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                    break;
                case 1:
                    setBoundingBox(0.0F, 12.0F / 16.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 2:
                    setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                    break;
                case 3:
                    setBoundingBox(0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 4:
                    setBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                    break;
                case 5:
                    setBoundingBox(12.0F / 16.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
            }

        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = func_31050_c(var1.getBlockMeta(var2, var3, var4));
            int var7 = var1.getBlockId(var2 - PistonBlockTextures.field_31056_b[var6], var3 - PistonBlockTextures.field_31059_c[var6], var4 - PistonBlockTextures.field_31058_d[var6]);
            if (var7 != Block.PISTON.id && var7 != Block.STICKY_PISTON.id)
            {
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }
            else
            {
                Block.BLOCKS[var7].neighborUpdate(var1, var2 - PistonBlockTextures.field_31056_b[var6], var3 - PistonBlockTextures.field_31059_c[var6], var4 - PistonBlockTextures.field_31058_d[var6], var5);
            }

        }

        public static int func_31050_c(int var0)
        {
            return var0 & 7;
        }
    }

}