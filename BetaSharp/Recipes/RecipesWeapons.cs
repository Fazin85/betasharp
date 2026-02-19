using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesWeapons
{
    private string[][] recipePatterns = [["X", "X", "#"]];
    private object[][] recipeItems = [[Block.Planks, Block.Cobblestone, Item.IronIngot, Item.Diamond, Item.GoldIngot], [Item.WoodenSword, Item.StoneSword, Item.IronSword, Item.DiamondSword, Item.GoldenSword]];

    public void AddRecipes(CraftingManager m)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            object material = recipeItems[0][i];
            for (int j = 0; j < recipePatterns.Length; ++j)
            {
                Item swordResult = (Item)recipeItems[j + 1][i];
                m.AddRecipe(new ItemStack(swordResult), recipePatterns[j], '#', Item.Stick, 'X', material);
            }
        }
            
        m.AddRecipe(new ItemStack(Item.BOW, 1), " #X", "# X", " #X", 'X', Item.String, '#', Item.Stick);
        m.AddRecipe(new ItemStack(Item.ARROW, 4), "X", "#", "Y", 'Y', Item.Feather, 'X', Item.Flint, '#', Item.Stick);
    }
}
