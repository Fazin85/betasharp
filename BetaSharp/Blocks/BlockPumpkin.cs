using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockPumpkin : Block
{
    private bool lit;

    // Textures calculées depuis terrain.png (base textureId=102 = pumpkin_top)
    // 102        = col6  row6 = pumpkin_top
    // 102+16=118 = col6  row7 = pumpkin_side
    // 102+17=119 = col7  row7 = pumpkin_face (non allumée)
    // 102+18=120 = col8  row7 = pumpkin_face_lit
    private const string TexTop = "pumpkin_top";
    private const string TexSide = "pumpkin_side";
    private const string TexFace = "pumpkin_face";
    private const string TexFaceLit = "pumpkin_face_lit";

    public BlockPumpkin(int id, string textureId, bool lit) : base(id, textureId, Material.Pumpkin)
    {
        setTickRandomly(true);
        this.lit = lit;
    }

    public override string getTexture(string side, int meta)
    {
        if (side == "top" || side == "bottom")
            return TexTop;

        // La face avant dépend de l'orientation (meta) et de la face demandée
        string faceTexture = lit ? TexFaceLit : TexFace;

        bool isFrontFace = (meta == 2 && side == "north")
                        || (meta == 3 && side == "west")
                        || (meta == 0 && side == "south")
                        || (meta == 1 && side == "east");

        return isFrontFace ? faceTexture : TexSide;
    }

    public override string getTexture(string side)
    {
        // Sans meta : face avant = sud par défaut
        if (side == "top" || side == "bottom") return TexTop;
        if (side == "south") return lit ? TexFaceLit : TexFace;
        return TexSide;
    }

    public override void onPlaced(World world, int x, int y, int z)
    {
        base.onPlaced(world, x, y, z);
    }

    public override bool canPlaceAt(World world, int x, int y, int z)
    {
        int blockId = world.getBlockId(x, y, z);
        return (blockId == 0 || Block.Blocks[blockId].material.IsReplaceable)
               && world.shouldSuffocate(x, y - 1, z);
    }

    public override void onPlaced(World world, int x, int y, int z, EntityLiving placer)
    {
        int direction = MathHelper.Floor(placer.yaw * 4.0F / 360.0F + 2.5D) & 3;
        world.setBlockMeta(x, y, z, direction);
    }
}
