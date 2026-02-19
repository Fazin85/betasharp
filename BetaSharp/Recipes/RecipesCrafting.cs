using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesCrafting
{
    public void AddRecipes(CraftingManager manager)
    {
        manager.AddRecipe(new ItemStack(Block.Chest), "###", "# #", "###", '#', Block.Planks);
        manager.AddRecipe(new ItemStack(Block.Furnace), "###", "# #", "###", '#', Block.Cobblestone);
        manager.AddRecipe(new ItemStack(Block.CraftingTable), "##", "##", '#', Block.Planks);
        manager.AddRecipe(new ItemStack(Block.Sandstone), "##", "##", '#', Block.Sand);
    }
}