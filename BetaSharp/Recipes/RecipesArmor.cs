using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesArmor
{
    private string[][] recipePatterns = [["XXX", "X X"], ["X X", "XXX", "XXX"], ["XXX", "X X", "X X"], ["X X", "X X"]];
    private object[][] recipeItems = [new object[] { Item.LEATHER, Block.FIRE, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT }, [Item.LEATHER_HELMET, Item.CHAIN_HELMET, Item.IRON_HELMET, Item.DIAMOND_HELMET, Item.GOLDEN_HELMET], [Item.LEATHER_CHESTPLATE, Item.CHAIN_CHESTPLATE, Item.IRON_CHESTPLATE, Item.DIAMOND_CHESTPLATE, Item.GOLDEN_CHESTPLATE], [Item.LEATHER_LEGGINGS, Item.CHAIN_LEGGINGS, Item.IRON_LEGGINGS, Item.DIAMOND_LEGGINGS, Item.GOLDEN_LEGGINGS], [Item.LEATHER_BOOTS, Item.CHAIN_BOOTS, Item.IRON_BOOTS, Item.DIAMOND_BOOTS, Item.GOLDEN_BOOTS]];

    public void addRecipes(CraftingManager var1)
    {
        for (int var2 = 0; var2 < recipeItems[0].Length; ++var2)
        {
            object var3 = recipeItems[0][var2];

            for (int var4 = 0; var4 < recipeItems.Length - 1; ++var4)
            {
                Item var5 = (Item)recipeItems[var4 + 1][var2];
                var1.addRecipe(new ItemStack(var5), [recipePatterns[var4], Character.valueOf('X'), var3]);
            }
        }

    }
}