using betareborn.Entities;
using betareborn.Materials;
using betareborn.TileEntities;
using betareborn.Worlds;
using java.util;

namespace betareborn.Blocks
{
    public class BlockPistonBase : Block
    {
        private bool isSticky;
        private bool field_31048_b;

        public BlockPistonBase(int var1, int var2, bool var3) : base(var1, var2, Material.PISTON)
        {
            isSticky = var3;
            setSoundGroup(soundStoneFootstep);
            setHardness(0.5F);
        }

        public int func_31040_i()
        {
            return isSticky ? 106 : 107;
        }

        public override int getTexture(int var1, int var2)
        {
            int var3 = func_31044_d(var2);
            return var3 > 5 ? textureId : (var1 == var3 ? (!isPowered(var2) && minX <= 0.0D && minY <= 0.0D && minZ <= 0.0D && maxX >= 1.0D && maxY >= 1.0D && maxZ >= 1.0D ? textureId : 110) : (var1 == PistonBlockTextures.field_31057_a[var3] ? 109 : 108));
        }

        public override int getRenderType()
        {
            return 16;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            return false;
        }

        public override void onBlockPlacedBy(World var1, int var2, int var3, int var4, EntityLiving var5)
        {
            int var6 = func_31039_c(var1, var2, var3, var4, (EntityPlayer)var5);
            var1.setBlockMeta(var2, var3, var4, var6);
            if (!var1.multiplayerWorld)
            {
                func_31043_h(var1, var2, var3, var4);
            }

        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (!var1.multiplayerWorld && !field_31048_b)
            {
                func_31043_h(var1, var2, var3, var4);
            }

        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            if (!var1.multiplayerWorld && var1.getBlockTileEntity(var2, var3, var4) == null)
            {
                func_31043_h(var1, var2, var3, var4);
            }

        }

        private void func_31043_h(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            int var6 = func_31044_d(var5);
            bool var7 = func_31041_f(var1, var2, var3, var4, var6);
            if (var5 != 7)
            {
                if (var7 && !isPowered(var5))
                {
                    if (func_31045_h(var1, var2, var3, var4, var6))
                    {
                        var1.setBlockMetadata(var2, var3, var4, var6 | 8);
                        var1.playNoteBlockActionAt(var2, var3, var4, 0, var6);
                    }
                }
                else if (!var7 && isPowered(var5))
                {
                    var1.setBlockMetadata(var2, var3, var4, var6);
                    var1.playNoteBlockActionAt(var2, var3, var4, 1, var6);
                }

            }
        }

        private bool func_31041_f(World var1, int var2, int var3, int var4, int var5)
        {
            return var5 != 0 && var1.isBlockIndirectlyProvidingPowerTo(var2, var3 - 1, var4, 0) ? true : (var5 != 1 && var1.isBlockIndirectlyProvidingPowerTo(var2, var3 + 1, var4, 1) ? true : (var5 != 2 && var1.isBlockIndirectlyProvidingPowerTo(var2, var3, var4 - 1, 2) ? true : (var5 != 3 && var1.isBlockIndirectlyProvidingPowerTo(var2, var3, var4 + 1, 3) ? true : (var5 != 5 && var1.isBlockIndirectlyProvidingPowerTo(var2 + 1, var3, var4, 5) ? true : (var5 != 4 && var1.isBlockIndirectlyProvidingPowerTo(var2 - 1, var3, var4, 4) ? true : (var1.isBlockIndirectlyProvidingPowerTo(var2, var3, var4, 0) ? true : (var1.isBlockIndirectlyProvidingPowerTo(var2, var3 + 2, var4, 1) ? true : (var1.isBlockIndirectlyProvidingPowerTo(var2, var3 + 1, var4 - 1, 2) ? true : (var1.isBlockIndirectlyProvidingPowerTo(var2, var3 + 1, var4 + 1, 3) ? true : (var1.isBlockIndirectlyProvidingPowerTo(var2 - 1, var3 + 1, var4, 4) ? true : var1.isBlockIndirectlyProvidingPowerTo(var2 + 1, var3 + 1, var4, 5)))))))))));
        }

        public override void playBlock(World var1, int var2, int var3, int var4, int var5, int var6)
        {
            field_31048_b = true;
            if (var5 == 0)
            {
                if (func_31047_i(var1, var2, var3, var4, var6))
                {
                    var1.setBlockMeta(var2, var3, var4, var6 | 8);
                    var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.5D, (double)var4 + 0.5D, "tile.piston.out", 0.5F, var1.random.nextFloat() * 0.25F + 0.6F);
                }
            }
            else if (var5 == 1)
            {
                TileEntity var8 = var1.getBlockTileEntity(var2 + PistonBlockTextures.field_31056_b[var6], var3 + PistonBlockTextures.field_31059_c[var6], var4 + PistonBlockTextures.field_31058_d[var6]);
                if (var8 != null && var8 is TileEntityPiston)
                {
                    ((TileEntityPiston)var8).finish();
                }

                var1.setBlockAndMetadata(var2, var3, var4, Block.MOVING_PISTON.id, var6);
                var1.setBlockTileEntity(var2, var3, var4, BlockPistonMoving.func_31036_a(id, var6, var6, false, true));
                if (isSticky)
                {
                    int var9 = var2 + PistonBlockTextures.field_31056_b[var6] * 2;
                    int var10 = var3 + PistonBlockTextures.field_31059_c[var6] * 2;
                    int var11 = var4 + PistonBlockTextures.field_31058_d[var6] * 2;
                    int var12 = var1.getBlockId(var9, var10, var11);
                    int var13 = var1.getBlockMeta(var9, var10, var11);
                    bool var14 = false;
                    if (var12 == Block.MOVING_PISTON.id)
                    {
                        TileEntity var15 = var1.getBlockTileEntity(var9, var10, var11);
                        if (var15 != null && var15 is TileEntityPiston)
                        {
                            TileEntityPiston var16 = (TileEntityPiston)var15;
                            if (var16.getFacing() == var6 && var16.isExtending())
                            {
                                var16.finish();
                                var12 = var16.getPushedBlockId();
                                var13 = var16.getPushedBlockData();
                                var14 = true;
                            }
                        }
                    }

                    if (var14 || var12 <= 0 || !canPushBlock(var12, var1, var9, var10, var11, false) || Block.BLOCKS[var12].getPistonBehavior() != 0 && var12 != Block.PISTON.id && var12 != Block.STICKY_PISTON.id)
                    {
                        if (!var14)
                        {
                            field_31048_b = false;
                            var1.setBlockWithNotify(var2 + PistonBlockTextures.field_31056_b[var6], var3 + PistonBlockTextures.field_31059_c[var6], var4 + PistonBlockTextures.field_31058_d[var6], 0);
                            field_31048_b = true;
                        }
                    }
                    else
                    {
                        field_31048_b = false;
                        var1.setBlockWithNotify(var9, var10, var11, 0);
                        field_31048_b = true;
                        var2 += PistonBlockTextures.field_31056_b[var6];
                        var3 += PistonBlockTextures.field_31059_c[var6];
                        var4 += PistonBlockTextures.field_31058_d[var6];
                        var1.setBlockAndMetadata(var2, var3, var4, Block.MOVING_PISTON.id, var13);
                        var1.setBlockTileEntity(var2, var3, var4, BlockPistonMoving.func_31036_a(var12, var13, var6, false, false));
                    }
                }
                else
                {
                    field_31048_b = false;
                    var1.setBlockWithNotify(var2 + PistonBlockTextures.field_31056_b[var6], var3 + PistonBlockTextures.field_31059_c[var6], var4 + PistonBlockTextures.field_31058_d[var6], 0);
                    field_31048_b = true;
                }

                var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.5D, (double)var4 + 0.5D, "tile.piston.in", 0.5F, var1.random.nextFloat() * 0.15F + 0.6F);
            }

            field_31048_b = false;
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            if (isPowered(var5))
            {
                switch (func_31044_d(var5))
                {
                    case 0:
                        setBoundingBox(0.0F, 0.25F, 0.0F, 1.0F, 1.0F, 1.0F);
                        break;
                    case 1:
                        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 12.0F / 16.0F, 1.0F);
                        break;
                    case 2:
                        setBoundingBox(0.0F, 0.0F, 0.25F, 1.0F, 1.0F, 1.0F);
                        break;
                    case 3:
                        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 12.0F / 16.0F);
                        break;
                    case 4:
                        setBoundingBox(0.25F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                        break;
                    case 5:
                        setBoundingBox(0.0F, 0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F);
                        break;
                }
            }
            else
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
            }

        }

        public override void setBlockBoundsForItemRender()
        {
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        }

        public override void addIntersectingBoundingBox(World var1, int var2, int var3, int var4, Box var5, List<Box> var6)
        {
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
            base.addIntersectingBoundingBox(var1, var2, var3, var4, var5, var6);
        }

        public override bool isFullCube()
        {
            return false;
        }

        public static int func_31044_d(int var0)
        {
            return var0 & 7;
        }

        public static bool isPowered(int var0)
        {
            return (var0 & 8) != 0;
        }

        private static int func_31039_c(World var0, int var1, int var2, int var3, EntityPlayer var4)
        {
            if (MathHelper.abs((float)var4.posX - (float)var1) < 2.0F && MathHelper.abs((float)var4.posZ - (float)var3) < 2.0F)
            {
                double var5 = var4.posY + 1.82D - (double)var4.yOffset;
                if (var5 - (double)var2 > 2.0D)
                {
                    return 1;
                }

                if ((double)var2 - var5 > 0.0D)
                {
                    return 0;
                }
            }

            int var7 = MathHelper.floor_double((double)(var4.rotationYaw * 4.0F / 360.0F) + 0.5D) & 3;
            return var7 == 0 ? 2 : (var7 == 1 ? 5 : (var7 == 2 ? 3 : (var7 == 3 ? 4 : 0)));
        }

        private static bool canPushBlock(int var0, World var1, int var2, int var3, int var4, bool var5)
        {
            if (var0 == Block.OBSIDIAN.id)
            {
                return false;
            }
            else
            {
                if (var0 != Block.PISTON.id && var0 != Block.STICKY_PISTON.id)
                {
                    if (Block.BLOCKS[var0].getHardness() == -1.0F)
                    {
                        return false;
                    }

                    if (Block.BLOCKS[var0].getPistonBehavior() == 2)
                    {
                        return false;
                    }

                    if (!var5 && Block.BLOCKS[var0].getPistonBehavior() == 1)
                    {
                        return false;
                    }
                }
                else if (isPowered(var1.getBlockMeta(var2, var3, var4)))
                {
                    return false;
                }

                TileEntity var6 = var1.getBlockTileEntity(var2, var3, var4);
                return var6 == null;
            }
        }

        private static bool func_31045_h(World var0, int var1, int var2, int var3, int var4)
        {
            int var5 = var1 + PistonBlockTextures.field_31056_b[var4];
            int var6 = var2 + PistonBlockTextures.field_31059_c[var4];
            int var7 = var3 + PistonBlockTextures.field_31058_d[var4];
            int var8 = 0;

            while (true)
            {
                if (var8 < 13)
                {
                    if (var6 <= 0 || var6 >= 127)
                    {
                        return false;
                    }

                    int var9 = var0.getBlockId(var5, var6, var7);
                    if (var9 != 0)
                    {
                        if (!canPushBlock(var9, var0, var5, var6, var7, true))
                        {
                            return false;
                        }

                        if (Block.BLOCKS[var9].getPistonBehavior() != 1)
                        {
                            if (var8 == 12)
                            {
                                return false;
                            }

                            var5 += PistonBlockTextures.field_31056_b[var4];
                            var6 += PistonBlockTextures.field_31059_c[var4];
                            var7 += PistonBlockTextures.field_31058_d[var4];
                            ++var8;
                            continue;
                        }
                    }
                }

                return true;
            }
        }

        private bool func_31047_i(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = var2 + PistonBlockTextures.field_31056_b[var5];
            int var7 = var3 + PistonBlockTextures.field_31059_c[var5];
            int var8 = var4 + PistonBlockTextures.field_31058_d[var5];
            int var9 = 0;

            while (true)
            {
                int var10;
                if (var9 < 13)
                {
                    if (var7 <= 0 || var7 >= 127)
                    {
                        return false;
                    }

                    var10 = var1.getBlockId(var6, var7, var8);
                    if (var10 != 0)
                    {
                        if (!canPushBlock(var10, var1, var6, var7, var8, true))
                        {
                            return false;
                        }

                        if (Block.BLOCKS[var10].getPistonBehavior() != 1)
                        {
                            if (var9 == 12)
                            {
                                return false;
                            }

                            var6 += PistonBlockTextures.field_31056_b[var5];
                            var7 += PistonBlockTextures.field_31059_c[var5];
                            var8 += PistonBlockTextures.field_31058_d[var5];
                            ++var9;
                            continue;
                        }

                        Block.BLOCKS[var10].dropBlockAsItem(var1, var6, var7, var8, var1.getBlockMeta(var6, var7, var8));
                        var1.setBlockWithNotify(var6, var7, var8, 0);
                    }
                }

                while (var6 != var2 || var7 != var3 || var8 != var4)
                {
                    var9 = var6 - PistonBlockTextures.field_31056_b[var5];
                    var10 = var7 - PistonBlockTextures.field_31059_c[var5];
                    int var11 = var8 - PistonBlockTextures.field_31058_d[var5];
                    int var12 = var1.getBlockId(var9, var10, var11);
                    int var13 = var1.getBlockMeta(var9, var10, var11);
                    if (var12 == id && var9 == var2 && var10 == var3 && var11 == var4)
                    {
                        var1.setBlockAndMetadata(var6, var7, var8, Block.MOVING_PISTON.id, var5 | (isSticky ? 8 : 0));
                        var1.setBlockTileEntity(var6, var7, var8, BlockPistonMoving.func_31036_a(Block.PISTON_HEAD.id, var5 | (isSticky ? 8 : 0), var5, true, false));
                    }
                    else
                    {
                        var1.setBlockAndMetadata(var6, var7, var8, Block.MOVING_PISTON.id, var13);
                        var1.setBlockTileEntity(var6, var7, var8, BlockPistonMoving.func_31036_a(var12, var13, var5, true, false));
                    }

                    var6 = var9;
                    var7 = var10;
                    var8 = var11;
                }

                return true;
            }
        }
    }

}