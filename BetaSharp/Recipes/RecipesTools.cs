using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesTools
{
    private string[][] recipePatterns = [["XXX", " # ", " # "], ["X", "#", "#"], ["XX", "X#", " #"], ["XX", " #", " #"]];
    private object[][] recipeItems = [[Block.Planks, Block.Cobblestone, Item.IronIngot, Item.Diamond, Item.GoldIngot], [Item.WoodenPickaxe, Item.StonePickaxe, Item.IronPickaxe, Item.DiamondPickaxe, Item.GoldenPickaxe], [Item.WoodenShovel, Item.StoneShovel, Item.IronShovel, Item.DiamondShovel, Item.GoldenShovel], [Item.WoodenAxe, Item.StoneAxe, Item.IronAxe, Item.DiamondAxe, Item.GoldenAxe], [Item.WoodenHoe, Item.StoneHoe, Item.IronHoe, Item.DiamondHoe, Item.GoldenHoe]];

    public void AddRecipes(CraftingManager m)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            var material = recipeItems[0][i];

            for (int j = 0; j < recipeItems.Length - 1; ++j)
            {
                Item toolItem = (Item)recipeItems[j + 1][i];
                m.AddRecipe(new ItemStack(toolItem), recipePatterns[j], '#', Item.Stick, 'X', material);
            }
        }

        m.AddRecipe(new ItemStack(Item.Shears), " #", "# ", '#', Item.IronIngot);
    }
}
