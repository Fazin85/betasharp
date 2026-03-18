using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockCake : Block
{
    // Textures fixes — le cake a 4 faces distinctes dans terrain.png
    private const string TexTop = "cake_top";
    private const string TexSide = "cake_side";
    private const string TexCut = "cake_side_cut";   // face entamée (côté ouest après 1+ tranche)
    private const string TexBottom = "cake_bottom";

    public BlockCake(int id, string textureId) : base(id, textureId, Material.Cake)
    {
        setTickRandomly(true);
    }

    public override string getTexture(string side, int meta)
    {
        return side == "top" ? TexTop
             : side == "bottom" ? TexBottom
             : meta > 0 && side == "west" ? TexCut
             : TexSide;
    }

    public override string getTexture(string side)
    {
        return side == "top" ? TexTop
             : side == "bottom" ? TexBottom
             : TexSide;
    }

    public override void updateBoundingBox(BlockView blockView, int x, int y, int z)
    {
        int slicesEaten = blockView.getBlockMeta(x, y, z);
        float edgeInset = 1.0F / 16.0F;
        float minX = (float)(1 + slicesEaten * 2) / 16.0F;
        float height = 0.5F;
        setBoundingBox(minX, 0.0F, edgeInset, 1.0F - edgeInset, height, 1.0F - edgeInset);
    }

    public override void setupRenderBoundingBox()
    {
        float edgeInset = 1.0F / 16.0F;
        float height = 0.5F;
        setBoundingBox(edgeInset, 0.0F, edgeInset, 1.0F - edgeInset, height, 1.0F - edgeInset);
    }

    public override Box? getCollisionShape(World world, int x, int y, int z)
    {
        int slicesEaten = world.getBlockMeta(x, y, z);
        float edgeInset = 1.0F / 16.0F;
        float minX = (float)(1 + slicesEaten * 2) / 16.0F;
        float height = 0.5F;
        return new Box(
            x + minX, y, z + edgeInset,
            x + 1 - edgeInset, y + height - edgeInset, z + 1 - edgeInset);
    }

    public override Box getBoundingBox(World world, int x, int y, int z)
    {
        int slicesEaten = world.getBlockMeta(x, y, z);
        float edgeInset = 1.0F / 16.0F;
        float minX = (float)(1 + slicesEaten * 2) / 16.0F;
        float height = 0.5F;
        return new Box(
            x + minX, y, z + edgeInset,
            x + 1 - edgeInset, y + height, z + 1 - edgeInset);
    }

    public override bool isFullCube() => false;
    public override bool isOpaque() => false;

    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        tryEat(world, x, y, z, player);
        return true;
    }

    public override void onBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
    {
        tryEat(world, x, y, z, player);
    }

    private void tryEat(World world, int x, int y, int z, EntityPlayer player)
    {
        if (player.health < 20)
        {
            player.heal(3);
            int slices = world.getBlockMeta(x, y, z) + 1;
            if (slices >= 6)
                world.setBlock(x, y, z, 0);
            else
            {
                world.setBlockMeta(x, y, z, slices);
                world.setBlocksDirty(x, y, z);
            }
        }
    }

    public override bool canPlaceAt(World world, int x, int y, int z)
        => base.canPlaceAt(world, x, y, z) && canGrow(world, x, y, z);

    public override void neighborUpdate(World world, int x, int y, int z, int id)
    {
        if (!canGrow(world, x, y, z))
        {
            dropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
            world.setBlock(x, y, z, 0);
        }
    }

    public override bool canGrow(World world, int x, int y, int z)
        => world.getMaterial(x, y - 1, z).IsSolid;

    public override int getDroppedItemCount(JavaRandom random) => 0;
    public override int getDroppedItemId(int blockMeta, JavaRandom random) => 0;
}
