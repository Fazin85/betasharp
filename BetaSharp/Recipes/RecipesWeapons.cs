using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesWeapons
{
    private string[][] recipePatterns = [["X", "X", "#"]];
    private object[][] recipeItems = [[Block.PLANKS, Block.COBBLESTONE, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT], [Item.WOODEN_SWORD, Item.STONE_SWORD, Item.IRON_SWORD, Item.DIAMOND_SWORD, Item.GOLDEN_SWORD]];

    public void addRecipes(CraftingManager var1)
    {
        for (int var2 = 0; var2 < recipeItems[0].Length; ++var2)
        {
            object var3 = recipeItems[0][var2];

            for (int var4 = 0; var4 < recipeItems.Length - 1; ++var4)
            {
                Item var5 = (Item)recipeItems[var4 + 1][var2];
                var1.addRecipe(new ItemStack(var5), [recipePatterns[var4], Character.valueOf('#'), Item.STICK, Character.valueOf('X'), var3]);
            }
        }

        var1.addRecipe(new ItemStack(Item.BOW, 1), [" #X", "# X", " #X", Character.valueOf('X'), Item.STRING, Character.valueOf('#'), Item.STICK]);
        var1.addRecipe(new ItemStack(Item.ARROW, 4), ["X", "#", "Y", Character.valueOf('Y'), Item.FEATHER, Character.valueOf('X'), Item.FLINT, Character.valueOf('#'), Item.STICK]);
    }
}