using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesIngots
{
    private object[][] recipeItems = [[Block.GoldBlock, new ItemStack(Item.GOLD_INGOT, 9)], [Block.IronBlock, new ItemStack(Item.IRON_INGOT, 9)], [Block.DiamondBlock, new ItemStack(Item.DIAMOND, 9)], [Block.LapisBlock, new ItemStack(Item.DYE, 9, 4)]];

    public void AddRecipes(CraftingManager m)
    {
        for (int i = 0; i < recipeItems.Length; ++i)
        {
            Block block = (Block)recipeItems[i][0];
            ItemStack ingot = (ItemStack)recipeItems[i][1];
            m.AddRecipe(new ItemStack(block), ["###", "###", "###", '#', ingot]);
            m.AddRecipe(ingot, ["#", '#', block]);
        }

    }
}