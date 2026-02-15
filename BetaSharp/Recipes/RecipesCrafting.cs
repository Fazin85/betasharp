using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesCrafting
{
    public void AddRecipes(CraftingManager manager)
    {
        manager.AddRecipe(new ItemStack(Block.CHEST), ["###", "# #", "###", '#', Block.PLANKS]);
        manager.AddRecipe(new ItemStack(Block.FURNACE), ["###", "# #", "###", '#', Block.COBBLESTONE]);
        manager.AddRecipe(new ItemStack(Block.CRAFTING_TABLE), ["##", "##", '#', Block.PLANKS]);
        manager.AddRecipe(new ItemStack(Block.SANDSTONE), ["##", "##", '#', Block.SAND]);
    }
}