using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemAxe : ItemTool
{

    private static Block[] blocksEffectiveAgainst = [
        Block.Planks, 
        Block.Bookshelf, 
        Block.Log, 
        Block.Chest,
        Block.CraftingTable,
        Block.Bed,
        Block.Pumpkin,
        Block.CarvedPumpkin,
        Block.JackLantern,
        Block.Jukebox,
        Block.Door,
        Block.WoodenStairs,
        Block.WoodenPressurePlate,
        Block.WallSign,
        Block.Trapdoor,
        Block.Sign,
        Block.Noteblock,
        Block.Fence,
        ];

    public ItemAxe(int id, EnumToolMaterial enumToolMaterial) : base(id, 3, enumToolMaterial, blocksEffectiveAgainst)
    {
    }
}