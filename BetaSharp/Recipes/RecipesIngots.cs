using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesIngots
{
    private object[][] recipeItems = [[Block.GOLD_BLOCK, new ItemStack(Item.GOLD_INGOT, 9)], [Block.IRON_BLOCK, new ItemStack(Item.IRON_INGOT, 9)], [Block.DIAMOND_BLOCK, new ItemStack(Item.DIAMOND, 9)], [Block.LAPIS_BLOCK, new ItemStack(Item.DYE, 9, 4)]];

    public void addRecipes(CraftingManager var1)
    {
        for (int var2 = 0; var2 < recipeItems.Length; ++var2)
        {
            Block var3 = (Block)recipeItems[var2][0];
            ItemStack var4 = (ItemStack)recipeItems[var2][1];
            var1.addRecipe(new ItemStack(var3), ["###", "###", "###", Character.valueOf('#'), var4]);
            var1.addRecipe(var4, ["#", Character.valueOf('#'), var3]);
        }

    }
}