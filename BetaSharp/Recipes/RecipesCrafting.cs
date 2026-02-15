using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesCrafting
{
    public void AddRecipes(CraftingManager manager)
    {
        manager.AddRecipe(new ItemStack(Block.CHEST), ["###", "# #", "###", Character.valueOf('#'), Block.PLANKS]);
        manager.AddRecipe(new ItemStack(Block.FURNACE), ["###", "# #", "###", Character.valueOf('#'), Block.COBBLESTONE]);
        manager.AddRecipe(new ItemStack(Block.CRAFTING_TABLE), ["##", "##", Character.valueOf('#'), Block.PLANKS]);
        manager.AddRecipe(new ItemStack(Block.SANDSTONE), ["##", "##", Character.valueOf('#'), Block.SAND]);
    }
}