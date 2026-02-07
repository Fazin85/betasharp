using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockRail : Block
    {

        private readonly bool isPowered;

        public static bool isRailBlockAt(World var0, int var1, int var2, int var3)
        {
            int var4 = var0.getBlockId(var1, var2, var3);
            return var4 == Block.RAIL.id || var4 == Block.POWERED_RAIL.id || var4 == Block.DETECTOR_RAIL.id;
        }

        public static bool isRailBlock(int var0)
        {
            return var0 == Block.RAIL.id || var0 == Block.POWERED_RAIL.id || var0 == Block.DETECTOR_RAIL.id;
        }

        public BlockRail(int var1, int var2, bool var3) : base(var1, var2, Material.PISTON_BREAKABLE)
        {
            isPowered = var3;
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 2.0F / 16.0F, 1.0F);
        }

        public bool getIsPowered()
        {
            return isPowered;
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return null;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override MovingObjectPosition collisionRayTrace(World var1, int var2, int var3, int var4, Vec3D var5, Vec3D var6)
        {
            updateBoundingBox(var1, var2, var3, var4);
            return base.collisionRayTrace(var1, var2, var3, var4, var5, var6);
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            if (var5 >= 2 && var5 <= 5)
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 10.0F / 16.0F, 1.0F);
            }
            else
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 2.0F / 16.0F, 1.0F);
            }

        }

        public override int getTexture(int var1, int var2)
        {
            if (isPowered)
            {
                if (id == Block.POWERED_RAIL.id && (var2 & 8) == 0)
                {
                    return textureId - 16;
                }
            }
            else if (var2 >= 6)
            {
                return textureId - 16;
            }

            return textureId;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override int getRenderType()
        {
            return 9;
        }

        public int quantityDropped(Random var1)
        {
            return 1;
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2, var3 - 1, var4);
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            if (!var1.multiplayerWorld)
            {
                func_4031_h(var1, var2, var3, var4, true);
            }

        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (!var1.multiplayerWorld)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                int var7 = var6;
                if (isPowered)
                {
                    var7 = var6 & 7;
                }

                bool var8 = false;
                if (!var1.shouldSuffocate(var2, var3 - 1, var4))
                {
                    var8 = true;
                }

                if (var7 == 2 && !var1.shouldSuffocate(var2 + 1, var3, var4))
                {
                    var8 = true;
                }

                if (var7 == 3 && !var1.shouldSuffocate(var2 - 1, var3, var4))
                {
                    var8 = true;
                }

                if (var7 == 4 && !var1.shouldSuffocate(var2, var3, var4 - 1))
                {
                    var8 = true;
                }

                if (var7 == 5 && !var1.shouldSuffocate(var2, var3, var4 + 1))
                {
                    var8 = true;
                }

                if (var8)
                {
                    dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
                else if (id == Block.POWERED_RAIL.id)
                {
                    bool var9 = var1.isBlockIndirectlyGettingPowered(var2, var3, var4) || var1.isBlockIndirectlyGettingPowered(var2, var3 + 1, var4);
                    var9 = var9 || func_27044_a(var1, var2, var3, var4, var6, true, 0) || func_27044_a(var1, var2, var3, var4, var6, false, 0);
                    bool var10 = false;
                    if (var9 && (var6 & 8) == 0)
                    {
                        var1.setBlockMeta(var2, var3, var4, var7 | 8);
                        var10 = true;
                    }
                    else if (!var9 && (var6 & 8) != 0)
                    {
                        var1.setBlockMeta(var2, var3, var4, var7);
                        var10 = true;
                    }

                    if (var10)
                    {
                        var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                        if (var7 == 2 || var7 == 3 || var7 == 4 || var7 == 5)
                        {
                            var1.notifyBlocksOfNeighborChange(var2, var3 + 1, var4, id);
                        }
                    }
                }
                else if (var5 > 0 && Block.BLOCKS[var5].canProvidePower() && !isPowered && RailLogic.getNAdjacentTracks(new RailLogic(this, var1, var2, var3, var4)) == 3)
                {
                    func_4031_h(var1, var2, var3, var4, false);
                }

            }
        }

        private void func_4031_h(World var1, int var2, int var3, int var4, bool var5)
        {
            if (!var1.multiplayerWorld)
            {
                (new RailLogic(this, var1, var2, var3, var4)).func_792_a(var1.isBlockIndirectlyGettingPowered(var2, var3, var4), var5);
            }
        }

        private bool func_27044_a(World var1, int var2, int var3, int var4, int var5, bool var6, int var7)
        {
            if (var7 >= 8)
            {
                return false;
            }
            else
            {
                int var8 = var5 & 7;
                bool var9 = true;
                switch (var8)
                {
                    case 0:
                        if (var6)
                        {
                            ++var4;
                        }
                        else
                        {
                            --var4;
                        }
                        break;
                    case 1:
                        if (var6)
                        {
                            --var2;
                        }
                        else
                        {
                            ++var2;
                        }
                        break;
                    case 2:
                        if (var6)
                        {
                            --var2;
                        }
                        else
                        {
                            ++var2;
                            ++var3;
                            var9 = false;
                        }

                        var8 = 1;
                        break;
                    case 3:
                        if (var6)
                        {
                            --var2;
                            ++var3;
                            var9 = false;
                        }
                        else
                        {
                            ++var2;
                        }

                        var8 = 1;
                        break;
                    case 4:
                        if (var6)
                        {
                            ++var4;
                        }
                        else
                        {
                            --var4;
                            ++var3;
                            var9 = false;
                        }

                        var8 = 0;
                        break;
                    case 5:
                        if (var6)
                        {
                            ++var4;
                            ++var3;
                            var9 = false;
                        }
                        else
                        {
                            --var4;
                        }

                        var8 = 0;
                        break;
                }

                return func_27043_a(var1, var2, var3, var4, var6, var7, var8) ? true : var9 && func_27043_a(var1, var2, var3 - 1, var4, var6, var7, var8);
            }
        }

        private bool func_27043_a(World var1, int var2, int var3, int var4, bool var5, int var6, int var7)
        {
            int var8 = var1.getBlockId(var2, var3, var4);
            if (var8 == Block.POWERED_RAIL.id)
            {
                int var9 = var1.getBlockMeta(var2, var3, var4);
                int var10 = var9 & 7;
                if (var7 == 1 && (var10 == 0 || var10 == 4 || var10 == 5))
                {
                    return false;
                }

                if (var7 == 0 && (var10 == 1 || var10 == 2 || var10 == 3))
                {
                    return false;
                }

                if ((var9 & 8) != 0)
                {
                    if (!var1.isBlockIndirectlyGettingPowered(var2, var3, var4) && !var1.isBlockIndirectlyGettingPowered(var2, var3 + 1, var4))
                    {
                        return func_27044_a(var1, var2, var3, var4, var9, var5, var6 + 1);
                    }

                    return true;
                }
            }

            return false;
        }

        public override int getPistonBehavior()
        {
            return 0;
        }

        public static bool isPoweredBlockRail(BlockRail var0)
        {
            return var0.isPowered;
        }
    }

}