using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesWeapons
{
    private string[][] recipePatterns = [["X", "X", "#"]];
    private object[][] recipeItems = [[Block.Planks, Block.Cobblestone, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT], [Item.WOODEN_SWORD, Item.STONE_SWORD, Item.IRON_SWORD, Item.DIAMOND_SWORD, Item.GOLDEN_SWORD]];

    public void AddRecipes(CraftingManager m)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            object material = recipeItems[0][i];
            for (int j = 0; j < recipePatterns.Length; ++j)
            {
                Item swordResult = (Item)recipeItems[j + 1][i];
                m.AddRecipe(new ItemStack(swordResult), 
                    [ recipePatterns[j], '#', Item.STICK, 'X', material ]);
            }
        }
            
        m.AddRecipe(new ItemStack(Item.BOW, 1), [" #X", "# X", " #X", 'X', Item.STRING, '#', Item.STICK]);
        m.AddRecipe(new ItemStack(Item.ARROW, 4), ["X", "#", "Y", 'Y', Item.FEATHER, 'X', Item.FLINT, '#', Item.STICK]);
    }
}