using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesArmor
{
    private string[][] recipePatterns = [["XXX", "X X"], ["X X", "XXX", "XXX"], ["XXX", "X X", "X X"], ["X X", "X X"]];
    private object[][] recipeItems = [new object[] { Item.LEATHER, Block.Fire, Item.IRON_INGOT, Item.DIAMOND, Item.GOLD_INGOT }, [Item.LEATHER_HELMET, Item.CHAIN_HELMET, Item.IRON_HELMET, Item.DIAMOND_HELMET, Item.GOLDEN_HELMET], [Item.LEATHER_CHESTPLATE, Item.CHAIN_CHESTPLATE, Item.IRON_CHESTPLATE, Item.DIAMOND_CHESTPLATE, Item.GOLDEN_CHESTPLATE], [Item.LEATHER_LEGGINGS, Item.CHAIN_LEGGINGS, Item.IRON_LEGGINGS, Item.DIAMOND_LEGGINGS, Item.GOLDEN_LEGGINGS], [Item.LEATHER_BOOTS, Item.CHAIN_BOOTS, Item.IRON_BOOTS, Item.DIAMOND_BOOTS, Item.GOLDEN_BOOTS]];

    public void AddRecipes(CraftingManager manager)
    {
        for (int i = 0; i < recipeItems[0].Length; ++i)
        {
            object material = recipeItems[0][i];

            for (int j = 0; j < recipeItems.Length - 1; ++j)
            {
                Item armorItem = (Item)recipeItems[j + 1][i];
                manager.AddRecipe(new ItemStack(armorItem), [recipePatterns[j], 'X', material]);
            }
        }

    }
}