using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesTools
{
    private string[][] recipePatterns = [["XXX", " # ", " # "], ["X", "#", "#"], ["XX", "X#", " #"], ["XX", " #", " #"]];
    private object[][] recipeItems = [[Block.Planks, Block.Cobblestone, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT], [Item.WOODEN_PICKAXE, Item.STONE_PICKAXE, Item.IRON_PICKAXE, Item.DIAMOND_PICKAXE, Item.GOLDEN_PICKAXE], [Item.WOODEN_SHOVEL, Item.STONE_SHOVEL, Item.IRON_SHOVEL, Item.DIAMOND_SHOVEL, Item.GOLDEN_SHOVEL], [Item.WOODEN_AXE, Item.STONE_AXE, Item.IRON_AXE, Item.DIAMOND_AXE, Item.GOLDEN_AXE], [Item.WOODEN_HOE, Item.STONE_HOE, Item.IRON_HOE, Item.DIAMOND_HOE, Item.GOLDEN_HOE]];

    public void AddRecipes(CraftingManager m)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            var material = recipeItems[0][i];

            for (int j = 0; j < recipeItems.Length - 1; ++j)
            {
                Item toolItem = (Item)recipeItems[j + 1][i];
                m.AddRecipe(new ItemStack(toolItem), [recipePatterns[j], '#', Item.STICK, 'X', material]);
            }
        }

        m.AddRecipe(new ItemStack(Item.SHEARS), [" #", "# ",'#', Item.IRON_INGOT]);
    }
}