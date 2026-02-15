using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesIngots
{
    private object[][] recipeItems = [[Block.GOLD_BLOCK, new ItemStack(Item.GOLD_INGOT, 9)], [Block.IRON_BLOCK, new ItemStack(Item.IRON_INGOT, 9)], [Block.DIAMOND_BLOCK, new ItemStack(Item.DIAMOND, 9)], [Block.LAPIS_BLOCK, new ItemStack(Item.DYE, 9, 4)]];

    public void AddRecipes(CraftingManager manager)
    {
        for (int i = 0; i < recipeItems.Length; ++i)
        {
            Block block = (Block)recipeItems[i][0];
            ItemStack ingot = (ItemStack)recipeItems[i][1];
            manager.AddRecipe(new ItemStack(block), ["###", "###", "###", Character.valueOf('#'), ingot]);
            manager.AddRecipe(ingot, ["#", Character.valueOf('#'), block]);
        }

    }
}