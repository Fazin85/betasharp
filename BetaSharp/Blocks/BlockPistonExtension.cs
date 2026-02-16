using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockPistonExtension : Block
{
    private int PistonHeadSprite = -1;

    public BlockPistonExtension(int Id, int TextureId) : base(Id, TextureId, Material.Piston)
    {
        setSoundGroup(SoundStoneFootstep);
        setHardness(0.5F);
    }

    public void setSprite(int Sprite)
    {
        PistonHeadSprite = Sprite;
    }

    public void clearSprite()
    {
        PistonHeadSprite = -1;
    }

    public override void onBreak(World World, int x, int y, int z)
    {
        base.onBreak(World, x, y, z);
        int Meta = World.getBlockMeta(x, y, z);
        int HeadFacing = PistonConstants.OppositeSide[getFacing(Meta)];
        x += PistonConstants.HeadOffsetX[HeadFacing];
        y += PistonConstants.HeadOffsetY[HeadFacing];
        z += PistonConstants.HeadOffsetZ[HeadFacing];
        int PistonBaseId = World.getBlockId(x, y, z);
        if (PistonBaseId == Block.Piston.id || PistonBaseId == Block.StickyPiston.id)
        {
            Meta = World.getBlockMeta(x, y, z);
            if (BlockPistonBase.isExtended(Meta))
            {
                Block.Blocks[PistonBaseId].dropStacks(World, x, y, z, Meta);
                World.setBlock(x, y, z, 0);
            }
        }

    }

    public override int getTexture(int Side, int Meta)
    {
        int Facing = getFacing(Meta);
        return Side == Facing ? (PistonHeadSprite >= 0 ? PistonHeadSprite : ((Meta & 8) != 0 ? textureId - 1 : textureId)) : (Side == PistonConstants.OppositeSide[Facing] ? 107 : 108);
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

    public override bool canPlaceAt(World World, int X, int Y, int Z)
    {
        return false;
    }

    public override bool canPlaceAt(World World, int X, int Y, int Z, int Side)
    {
        return false;
    }

    public override int getDroppedItemCount(java.util.Random Random)
    {
        return 0;
    }

    public override void addIntersectingBoundingBox(World World, int X, int Y, int Z, Box Box, List<Box> Boxes)
    {
        int Meta = World.getBlockMeta(X, Y, Z);
        switch (getFacing(Meta))
        {
            case 0:
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                setBoundingBox(6.0F / 16.0F, 0.25F, 6.0F / 16.0F, 10.0F / 16.0F, 1.0F, 10.0F / 16.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                break;
            case 1:
                setBoundingBox(0.0F, 12.0F / 16.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                setBoundingBox(6.0F / 16.0F, 0.0F, 6.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F, 10.0F / 16.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                break;
            case 2:
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                setBoundingBox(0.25F, 6.0F / 16.0F, 0.25F, 12.0F / 16.0F, 10.0F / 16.0F, 1.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                break;
            case 3:
                setBoundingBox(0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F, 1.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                setBoundingBox(0.25F, 6.0F / 16.0F, 0.0F, 12.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                break;
            case 4:
                setBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                setBoundingBox(6.0F / 16.0F, 0.25F, 0.25F, 10.0F / 16.0F, 12.0F / 16.0F, 1.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                break;
            case 5:
                setBoundingBox(12.0F / 16.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                setBoundingBox(0.0F, 6.0F / 16.0F, 0.25F, 12.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F);
                base.addIntersectingBoundingBox(World, X, Y, Z, Box, Boxes);
                break;
        }

        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
    }

    public override void updateBoundingBox(BlockView BlockView, int X, int Y, int Z)
    {
        int Meta = BlockView.getBlockMeta(X, Y, Z);
        switch (getFacing(Meta))
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

    public override void neighborUpdate(World World, int X, int Y, int Z, int id)
    {
        int Facing = getFacing(World.getBlockMeta(X, Y, Z));
        int BaseId = World.getBlockId(X - PistonConstants.HeadOffsetX[Facing], Y - PistonConstants.HeadOffsetY[Facing], Z - PistonConstants.HeadOffsetZ[Facing]);
        if (BaseId != Block.Piston.id && BaseId != Block.StickyPiston.id)
        {
            World.setBlock(X, Y, Z, 0);
        }
        else
        {
            Block.Blocks[BaseId].neighborUpdate(World, X - PistonConstants.HeadOffsetX[Facing], Y - PistonConstants.HeadOffsetY[Facing], Z - PistonConstants.HeadOffsetZ[Facing], id);
        }

    }

    public static int getFacing(int Meta)
    {
        return Meta & 7;
    }
}