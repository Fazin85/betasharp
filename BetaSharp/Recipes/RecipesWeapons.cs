using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesWeapons
{
    private string[][] recipePatterns = [["X", "X", "#"]];
    private object[][] recipeItems = [[Block.PLANKS, Block.COBBLESTONE, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT], [Item.WOODEN_SWORD, Item.STONE_SWORD, Item.IRON_SWORD, Item.DIAMOND_SWORD, Item.GOLDEN_SWORD]];

    public void AddRecipes(CraftingManager manager)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            var material = recipeItems[0][i];

            for (int j = 0; j < recipeItems.Length - 1; ++j)
            {
                Item weaponItem = (Item)recipeItems[j + 1][i];
                manager.AddRecipe(new ItemStack(weaponItem), [recipePatterns[j], Character.valueOf('#'), Item.STICK, Character.valueOf('X'), material]);
            }
        }

        manager.AddRecipe(new ItemStack(Item.BOW, 1), [" #X", "# X", " #X", Character.valueOf('X'), Item.STRING, Character.valueOf('#'), Item.STICK]);
        manager.AddRecipe(new ItemStack(Item.ARROW, 4), ["X", "#", "Y", Character.valueOf('Y'), Item.FEATHER, Character.valueOf('X'), Item.FLINT, Character.valueOf('#'), Item.STICK]);
    }
}