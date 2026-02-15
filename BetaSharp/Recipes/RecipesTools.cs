using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesTools : java.lang.Object
{
    private string[][] recipePatterns = [["XXX", " # ", " # "], ["X", "#", "#"], ["XX", "X#", " #"], ["XX", " #", " #"]];
    private object[][] recipeItems = [[Block.PLANKS, Block.COBBLESTONE, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT], [Item.WOODEN_PICKAXE, Item.STONE_PICKAXE, Item.IRON_PICKAXE, Item.DIAMOND_PICKAXE, Item.GOLDEN_PICKAXE], [Item.WOODEN_SHOVEL, Item.STONE_SHOVEL, Item.IRON_SHOVEL, Item.DIAMOND_SHOVEL, Item.GOLDEN_SHOVEL], [Item.WOODEN_AXE, Item.STONE_AXE, Item.IRON_AXE, Item.DIAMOND_AXE, Item.GOLDEN_AXE], [Item.WOODEN_HOE, Item.STONE_HOE, Item.IRON_HOE, Item.DIAMOND_HOE, Item.GOLDEN_HOE]];

    public void AddRecipes(CraftingManager manager)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            var material = recipeItems[0][i];

            for (int j = 0; j < recipeItems.Length - 1; ++j)
            {
                Item toolItem = (Item)recipeItems[j + 1][i];
                manager.AddRecipe(new ItemStack(toolItem), [recipePatterns[j], Character.valueOf('#'), Item.STICK, Character.valueOf('X'), material]);
            }
        }

        manager.AddRecipe(new ItemStack(Item.SHEARS), [" #", "# ", Character.valueOf('#'), Item.IRON_INGOT]);
    }
}