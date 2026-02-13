using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesCrafting
{
    public void addRecipes(CraftingManager var1)
    {
        var1.addRecipe(new ItemStack(Block.CHEST), ["###", "# #", "###", Character.valueOf('#'), Block.PLANKS]);
        var1.addRecipe(new ItemStack(Block.FURNACE), ["###", "# #", "###", Character.valueOf('#'), Block.COBBLESTONE]);
        var1.addRecipe(new ItemStack(Block.CRAFTING_TABLE), ["##", "##", Character.valueOf('#'), Block.PLANKS]);
        var1.addRecipe(new ItemStack(Block.SANDSTONE), ["##", "##", Character.valueOf('#'), Block.SAND]);
    }
}