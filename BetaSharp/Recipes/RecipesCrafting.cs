using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesCrafting
{
    public void AddRecipes(CraftingManager manager)
    {
        var1.addRecipe(new ItemStack(Block.Chest), ["###", "# #", "###", Character.valueOf('#'), Block.Planks]);
        var1.addRecipe(new ItemStack(Block.Furnace), ["###", "# #", "###", Character.valueOf('#'), Block.Cobblestone]);
        var1.addRecipe(new ItemStack(Block.CraftingTable), ["##", "##", Character.valueOf('#'), Block.Planks]);
        var1.addRecipe(new ItemStack(Block.Sandstone), ["##", "##", Character.valueOf('#'), Block.Sand]);
    }
}