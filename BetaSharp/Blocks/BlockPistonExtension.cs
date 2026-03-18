using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockPistonExtension : Block
{
    // pistonHeadSprite : vestige du renderer legacy, gardé pour compatibilité
    // mais ignoré dans le nouveau système atlas
    private string pistonHeadSprite = "";

    public BlockPistonExtension(int id, string textureId) : base(id, textureId, Material.Piston)
    {
        setSoundGroup(SoundStoneFootstep);
        setHardness(0.5F);
    }

    public void setSprite(string sprite) => pistonHeadSprite = sprite;
    public void clearSprite() => pistonHeadSprite = "";

    // ── Textures ──────────────────────────────────────────────────────────
    // meta & 7 = direction (0=down 1=up 2=north 3=south 4=west 5=east)
    // meta & 8 = sticky (1 = tête sticky, 0 = tête normale)

    public override string getTexture(string side, int meta)
    {
        string facing = GetFacingString(meta);
        string opposite = PistonConstants.GetOpposite(facing);

        if (side == facing)
            // Face avant = la tête du piston
            return (meta & 8) != 0 ? "piston_top_sticky" : "piston_top";

        if (side == opposite)
            // Face arrière = dos plat du corps
            return "piston_top";

        // Les 4 faces latérales = le bras/tige
        return "piston_side";
    }

    public override string getTexture(string side)
    {
        // Sans meta (ex: inventaire) — piston rétracté, orientation up par défaut
        return side == "up" ? textureId   // "piston_top" ou "piston_top_sticky"
             : side == "down" ? "piston_top"
             : "piston_side";
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    public static int getFacing(int meta) => meta & 7;

    private static string GetFacingString(int meta) => (meta & 7) switch
    {
        0 => "down",
        1 => "up",
        2 => "north",
        3 => "south",
        4 => "west",
        5 => "east",
        _ => "up"
    };

    // ── Logique bloc ──────────────────────────────────────────────────────

    public override void onBreak(World world, int x, int y, int z)
    {
        base.onBreak(world, x, y, z);
        int meta = world.getBlockMeta(x, y, z);
        int oppFace = PistonConstants.field_31057_a[getFacing(meta)];
        x += PistonConstants.HEAD_OFFSET_X[oppFace];
        y += PistonConstants.HEAD_OFFSET_Y[oppFace];
        z += PistonConstants.HEAD_OFFSET_Z[oppFace];
        int neighborId = world.getBlockId(x, y, z);
        if (neighborId == Block.Piston.id || neighborId == Block.StickyPiston.id)
        {
            int neighborMeta = world.getBlockMeta(x, y, z);
            if (BlockPistonBase.isExtended(neighborMeta))
            {
                Block.Blocks[neighborId].dropStacks(world, x, y, z, neighborMeta);
                world.setBlock(x, y, z, 0);
            }
        }
    }

    public override void neighborUpdate(World world, int x, int y, int z, int id)
    {
        int facing = getFacing(world.getBlockMeta(x, y, z));
        int baseId = world.getBlockId(
            x - PistonConstants.HEAD_OFFSET_X[facing],
            y - PistonConstants.HEAD_OFFSET_Y[facing],
            z - PistonConstants.HEAD_OFFSET_Z[facing]);

        if (baseId != Block.Piston.id && baseId != Block.StickyPiston.id)
        {
            world.setBlock(x, y, z, 0);
        }
        else
        {
            Block.Blocks[baseId].neighborUpdate(world,
                x - PistonConstants.HEAD_OFFSET_X[facing],
                y - PistonConstants.HEAD_OFFSET_Y[facing],
                z - PistonConstants.HEAD_OFFSET_Z[facing], id);
        }
    }

    public override void addIntersectingBoundingBox(World world, int x, int y, int z, Box box, List<Box> boxes)
    {
        int facing = getFacing(world.getBlockMeta(x, y, z));
        switch (facing)
        {
            case 0:
                setBoundingBox(0f, 0f, 0f, 1f, 0.25f, 1f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(6f / 16, 0.25f, 6f / 16, 10f / 16, 1f, 10f / 16);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                break;
            case 1:
                setBoundingBox(0f, 12f / 16, 0f, 1f, 1f, 1f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(6f / 16, 0f, 6f / 16, 10f / 16, 12f / 16, 10f / 16);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                break;
            case 2:
                setBoundingBox(0f, 0f, 0f, 1f, 1f, 0.25f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(0.25f, 6f / 16, 0.25f, 12f / 16, 10f / 16, 1f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                break;
            case 3:
                setBoundingBox(0f, 0f, 12f / 16, 1f, 1f, 1f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(0.25f, 6f / 16, 0f, 12f / 16, 10f / 16, 12f / 16);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                break;
            case 4:
                setBoundingBox(0f, 0f, 0f, 0.25f, 1f, 1f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(6f / 16, 0.25f, 0.25f, 10f / 16, 12f / 16, 1f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                break;
            case 5:
                setBoundingBox(12f / 16, 0f, 0f, 1f, 1f, 1f);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(0f, 6f / 16, 0.25f, 12f / 16, 10f / 16, 12f / 16);
                base.addIntersectingBoundingBox(world, x, y, z, box, boxes);
                break;
        }
        setBoundingBox(0f, 0f, 0f, 1f, 1f, 1f);
    }

    public override void updateBoundingBox(BlockView blockView, int x, int y, int z)
    {
        switch (getFacing(blockView.getBlockMeta(x, y, z)))
        {
            case 0: setBoundingBox(0f, 0f, 0f, 1f, 0.25f, 1f); break;
            case 1: setBoundingBox(0f, 12f / 16, 0f, 1f, 1f, 1f); break;
            case 2: setBoundingBox(0f, 0f, 0f, 1f, 1f, 0.25f); break;
            case 3: setBoundingBox(0f, 0f, 12f / 16, 1f, 1f, 1f); break;
            case 4: setBoundingBox(0f, 0f, 0f, 0.25f, 1f, 1f); break;
            case 5: setBoundingBox(12f / 16, 0f, 0f, 1f, 1f, 1f); break;
        }
    }

    public override int getRenderType() => 17;
    public override bool isOpaque() => false;
    public override bool isFullCube() => false;
    public override bool canPlaceAt(World world, int x, int y, int z) => false;
    public override bool canPlaceAt(World world, int x, int y, int z, int side) => false;
    public override int getDroppedItemCount(JavaRandom random) => 0;
}
