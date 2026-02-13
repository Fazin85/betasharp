using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesTools : java.lang.Object
{
    private string[][] recipePatterns = [["XXX", " # ", " # "], ["X", "#", "#"], ["XX", "X#", " #"], ["XX", " #", " #"]];
    private object[][] recipeItems = [[Block.PLANKS, Block.COBBLESTONE, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT], [Item.WOODEN_PICKAXE, Item.STONE_PICKAXE, Item.IRON_PICKAXE, Item.DIAMOND_PICKAXE, Item.GOLDEN_PICKAXE], [Item.WOODEN_SHOVEL, Item.STONE_SHOVEL, Item.IRON_SHOVEL, Item.DIAMOND_SHOVEL, Item.GOLDEN_SHOVEL], [Item.WOODEN_AXE, Item.STONE_AXE, Item.IRON_AXE, Item.DIAMOND_AXE, Item.GOLDEN_AXE], [Item.WOODEN_HOE, Item.STONE_HOE, Item.IRON_HOE, Item.DIAMOND_HOE, Item.GOLDEN_HOE]];

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

        var1.addRecipe(new ItemStack(Item.SHEARS), [" #", "# ", Character.valueOf('#'), Item.IRON_INGOT]);
    }
}