using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockWorkbench : Block
{

    public BlockWorkbench(int id) : base(id, Material.Wood)
    {
        textureId = "workbench";
    }

    public override string getTexture(string side)
    {
        return side == "up" ? $"{textureId}_top"
            : side == "down" ? $"{textureId}_bottom"
            : $"{textureId}_side";
    }


    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        if (world.isRemote)
        {
            return true;
        }
        else
        {
            player.openCraftingScreen(x, y, z);
            return true;
        }
    }
}
