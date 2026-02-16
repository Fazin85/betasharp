using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockPistonBase : Block
{
    private bool IsSticky;
    private bool IsDeaf;

    public BlockPistonBase(int Id, int TextureId, bool IsSticky) : base(Id, TextureId, Material.Piston)
    {
        this.IsSticky = IsSticky;
        setSoundGroup(SoundStoneFootstep);
        setHardness(0.5F);
    }

    public int getTopTexture()
    {
        return IsSticky ? 106 : 107;
    }

    public override int getTexture(int side, int meta)
    {
        int var3 = getFacing(meta); 
        return var3 > 5 ? textureId : (side == var3 ? (!isExtended(meta) && minX <= 0.0D && minY <= 0.0D && minZ <= 0.0D && maxX >= 1.0D && maxY >= 1.0D && maxZ >= 1.0D ? textureId : 110) : (side == PistonConstants.OppositeSide[var3] ? 109 : 108));
    }

    public override int getRenderType()
    {
        return 16;
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override bool onUse(World World, int X, int Y, int Z, EntityPlayer Player)
    {
        return false;
    }

    public override void onPlaced(World World, int X, int Y, int Z, EntityLiving Placer)
    {
        int var6 = getFacingForPlacement(World, X, Y, Z, (EntityPlayer)Placer);
        World.setBlockMeta(X, Y, Z, var6);
        if (!World.isRemote)
        {
            checkExtended(World, X, Y, Z);
        }

    }

    public override void neighborUpdate(World World, int X, int Y, int Z, int Id)
    {
        if (!World.isRemote && !IsDeaf)
        {
            checkExtended(World, X, Y, Z);
        }

    }

    public override void onPlaced(World World, int X, int Y, int Z)
    {
        if (!World.isRemote && World.getBlockEntity(X, Y, Z) == null)
        {
            checkExtended(World, X, Y, Z);
        }

    }

    private void checkExtended(World World, int X, int Y, int Z)
    {
        int var5 = World.getBlockMeta(X, Y, Z);
        int var6 = getFacing(var5);
        bool var7 = shouldExtend(World, X, Y, Z, var6);
        if (var5 != 7)
        {
            if (var7 && !isExtended(var5))
            {
                if (canExtend(World, X, Y, Z, var6))
                {
                    World.SetBlockMetaWithoutNotifyingNeighbors(X, Y, Z, var6 | 8);
                    World.playNoteBlockActionAt(X, Y, Z, 0, var6);
                }
            }
            else if (!var7 && isExtended(var5))
            {
                World.SetBlockMetaWithoutNotifyingNeighbors(X, Y, Z, var6);
                World.playNoteBlockActionAt(X, Y, Z, 1, var6);
            }

        }
    }

    private bool shouldExtend(World World, int X, int Y, int Z, int Facing)
    {
        return Facing != 0 && World.isPoweringSide(X, Y - 1, Z, 0) ? true : (Facing != 1 && World.isPoweringSide(X, Y + 1, Z, 1) ? true : (Facing != 2 && World.isPoweringSide(X, Y, Z - 1, 2) ? true : (Facing != 3 && World.isPoweringSide(X, Y, Z + 1, 3) ? true : (Facing != 5 && World.isPoweringSide(X + 1, Y, Z, 5) ? true : (Facing != 4 && World.isPoweringSide(X - 1, Y, Z, 4) ? true : (World.isPoweringSide(X, Y, Z, 0) ? true : (World.isPoweringSide(X, Y + 2, Z, 1) ? true : (World.isPoweringSide(X, Y + 1, Z - 1, 2) ? true : (World.isPoweringSide(X, Y + 1, Z + 1, 3) ? true : (World.isPoweringSide(X - 1, Y + 1, Z, 4) ? true : World.isPoweringSide(X + 1, Y + 1, Z, 5)))))))))));
    }

    public override void onBlockAction(World World, int X, int Y, int Z, int data1, int data2)
    {
        IsDeaf = true;
        if (data1 == 0)
        {
            if (push(World, X, Y, Z, data2))
            {
                World.setBlockMeta(X, Y, Z, data2 | 8);
                World.playSound((double)X + 0.5D, (double)Y + 0.5D, (double)Z + 0.5D, "tile.piston.out", 0.5F, World.random.nextFloat() * 0.25F + 0.6F);
            }
        }
        else if (data1 == 1)
        {
            BlockEntity var8 = World.getBlockEntity(X + PistonConstants.HeadOffsetX[data2], Y + PistonConstants.HeadOffsetY[data2], Z + PistonConstants.HeadOffsetZ[data2]);
            if (var8 != null && var8 is BlockEntityPiston)
            {
                ((BlockEntityPiston)var8).finish();
            }

            World.SetBlockWithoutNotifyingNeighbors(X, Y, Z, MovingPiston.id, data2);
            World.setBlockEntity(X, Y, Z, BlockPistonMoving.createPistonBlockEntity(id, data2, data2, false, true));
            if (IsSticky)
            {
                int var9 = X + PistonConstants.HeadOffsetX[data2] * 2;
                int var10 = Y + PistonConstants.HeadOffsetY[data2] * 2;
                int var11 = Z + PistonConstants.HeadOffsetZ[data2] * 2;
                int var12 = World.getBlockId(var9, var10, var11);
                int var13 = World.getBlockMeta(var9, var10, var11);
                bool var14 = false;
                if (var12 == MovingPiston.id)
                {
                    BlockEntity var15 = World.getBlockEntity(var9, var10, var11);
                    if (var15 != null && var15 is BlockEntityPiston)
                    {
                        BlockEntityPiston var16 = (BlockEntityPiston)var15;
                        if (var16.getFacing() == data2 && var16.isExtending())
                        {
                            var16.finish();
                            var12 = var16.getPushedBlockId();
                            var13 = var16.getPushedBlockData();
                            var14 = true;
                        }
                    }
                }

                if (var14 || var12 <= 0 || !canMoveBlock(var12, World, var9, var10, var11, false) || Block.Blocks[var12].getPistonBehavior() != 0 && var12 != Block.Piston.id && var12 != Block.StickyPiston.id)
                {
                    if (!var14)
                    {
                        IsDeaf = false;
                        World.setBlock(X + PistonConstants.HeadOffsetX[data2], Y + PistonConstants.HeadOffsetY[data2], Z + PistonConstants.HeadOffsetZ[data2], 0);
                        IsDeaf = true;
                    }
                }
                else
                {
                    IsDeaf = false;
                    World.setBlock(var9, var10, var11, 0);
                    IsDeaf = true;
                    X += PistonConstants.HeadOffsetX[data2];
                    Y += PistonConstants.HeadOffsetY[data2];
                    Z += PistonConstants.HeadOffsetZ[data2];
                    World.SetBlockWithoutNotifyingNeighbors(X, Y, Z, MovingPiston.id, var13);
                    World.setBlockEntity(X, Y, Z, BlockPistonMoving.createPistonBlockEntity(var12, var13, data2, false, false));
                }
            }
            else
            {
                IsDeaf = false;
                World.setBlock(X + PistonConstants.HeadOffsetX[data2], Y + PistonConstants.HeadOffsetY[data2], Z + PistonConstants.HeadOffsetZ[data2], 0);
                IsDeaf = true;
            }

            World.playSound((double)X + 0.5D, (double)Y + 0.5D, (double)Z + 0.5D, "tile.piston.in", 0.5F, World.random.nextFloat() * 0.15F + 0.6F);
        }

        IsDeaf = false;
    }

    public override void updateBoundingBox(BlockView BlockView, int X, int Y, int Z)
    {
        int var5 = BlockView.getBlockMeta(X, Y, Z);
        if (isExtended(var5))
        {
            switch (getFacing(var5))
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

    public override void setupRenderBoundingBox()
    {
        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
    }

    public override void addIntersectingBoundingBox(World World, int X, int Y, int Z, Box box, List<Box> boxes)
    {
        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        base.addIntersectingBoundingBox(World, X, Y, Z, box, boxes);
    }

    public override bool isFullCube()
    {
        return false;
    }

    public static int getFacing(int Meta)
    {
        return Meta & 7;
    }

    public static bool isExtended(int Meta)
    {
        return (Meta & 8) != 0;
    }

    private static int getFacingForPlacement(World World, int X, int Y, int Z, EntityPlayer Player)
    {
        if (MathHelper.abs((float)Player.x - (float)X) < 2.0F && MathHelper.abs((float)Player.z - (float)Z) < 2.0F)
        {
            double var5 = Player.y + 1.82D - (double)Player.standingEyeHeight;
            if (var5 - (double)Y > 2.0D)
            {
                return 1;
            }

            if ((double)Y - var5 > 0.0D)
            {
                return 0;
            }
        }

        int var7 = MathHelper.floor_double((double)(Player.yaw * 4.0F / 360.0F) + 0.5D) & 3;
        return var7 == 0 ? 2 : (var7 == 1 ? 5 : (var7 == 2 ? 3 : (var7 == 3 ? 4 : 0)));
    }

    private static bool canMoveBlock(int Id, World World, int X, int Y, int Z, bool AllowBreaking)
    {
        if (Id == Block.Obsidian.id)
        {
            return false;
        }
        else
        {
            if (Id != Block.Piston.id && Id != Block.StickyPiston.id)
            {
                if (Block.Blocks[Id].getHardness() == -1.0F)
                {
                    return false;
                }

                if (Block.Blocks[Id].getPistonBehavior() == 2)
                {
                    return false;
                }

                if (!AllowBreaking && Block.Blocks[Id].getPistonBehavior() == 1)
                {
                    return false;
                }
            }
            else if (isExtended(World.getBlockMeta(X, Y, Z)))
            {
                return false;
            }

            BlockEntity var6 = World.getBlockEntity(X, Y, Z);
            return var6 == null;
        }
    }

    private static bool canExtend(World World, int X, int Y, int Z, int dir)
    {
        int var5 = X + PistonConstants.HeadOffsetX[dir];
        int var6 = Y + PistonConstants.HeadOffsetY[dir];
        int var7 = Z + PistonConstants.HeadOffsetZ[dir];
        int var8 = 0;

        while (true)
        {
            if (var8 < 13)
            {
                if (var6 <= 0 || var6 >= 127)
                {
                    return false;
                }

                int var9 = World.getBlockId(var5, var6, var7);
                if (var9 != 0)
                {
                    if (!canMoveBlock(var9, World, var5, var6, var7, true))
                    {
                        return false;
                    }

                    if (Block.Blocks[var9].getPistonBehavior() != 1)
                    {
                        if (var8 == 12)
                        {
                            return false;
                        }

                        var5 += PistonConstants.HeadOffsetX[dir];
                        var6 += PistonConstants.HeadOffsetY[dir];
                        var7 += PistonConstants.HeadOffsetZ[dir];
                        ++var8;
                        continue;
                    }
                }
            }

            return true;
        }
    }

    private bool push(World World, int X, int Y, int Z, int dir)
    {
        int var6 = X + PistonConstants.HeadOffsetX[dir];
        int var7 = Y + PistonConstants.HeadOffsetY[dir];
        int var8 = Z + PistonConstants.HeadOffsetZ[dir];
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

                var10 = World.getBlockId(var6, var7, var8);
                if (var10 != 0)
                {
                    if (!canMoveBlock(var10, World, var6, var7, var8, true))
                    {
                        return false;
                    }

                    if (Block.Blocks[var10].getPistonBehavior() != 1)
                    {
                        if (var9 == 12)
                        {
                            return false;
                        }

                        var6 += PistonConstants.HeadOffsetX[dir];
                        var7 += PistonConstants.HeadOffsetY[dir];
                        var8 += PistonConstants.HeadOffsetZ[dir];
                        ++var9;
                        continue;
                    }

                    Block.Blocks[var10].dropStacks(World, var6, var7, var8, World.getBlockMeta(var6, var7, var8));
                    World.setBlock(var6, var7, var8, 0);
                }
            }

            while (var6 != X || var7 != Y || var8 != Z)
            {
                var9 = var6 - PistonConstants.HeadOffsetX[dir];
                var10 = var7 - PistonConstants.HeadOffsetY[dir];
                int var11 = var8 - PistonConstants.HeadOffsetZ[dir];
                int var12 = World.getBlockId(var9, var10, var11);
                int var13 = World.getBlockMeta(var9, var10, var11);
                if (var12 == id && var9 == X && var10 == Y && var11 == Z)
                {
                    World.SetBlockWithoutNotifyingNeighbors(var6, var7, var8, MovingPiston.id, dir | (IsSticky ? 8 : 0));
                    World.setBlockEntity(var6, var7, var8, BlockPistonMoving.createPistonBlockEntity(PistonHead.id, dir | (IsSticky ? 8 : 0), dir, true, false));
                }
                else
                {
                    World.SetBlockWithoutNotifyingNeighbors(var6, var7, var8, MovingPiston.id, var13);
                    World.setBlockEntity(var6, var7, var8, BlockPistonMoving.createPistonBlockEntity(var12, var13, dir, true, false));
                }

                var6 = var9;
                var7 = var10;
                var8 = var11;
            }

            return true;
        }
    }
}