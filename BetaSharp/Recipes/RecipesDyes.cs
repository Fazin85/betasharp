using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesDyes
{
    public void addRecipes(CraftingManager var1)
    {
        for (int var2 = 0; var2 < 16; ++var2)
        {
            var1.addShapelessRecipe(new ItemStack(Block.WOOL, 1, BlockCloth.getItemMeta(var2)), [new ItemStack(Item.DYE, 1, var2), new ItemStack(Item.ITEMS[Block.WOOL.id], 1, 0)]);
        }

        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 11), new object[] { Block.DANDELION });
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 1), new object[] { Block.ROSE });
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 3, 15), [Item.BONE]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 9), [new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 15)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 14), [new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 11)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 10), [new ItemStack(Item.DYE, 1, 2), new ItemStack(Item.DYE, 1, 15)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 8), [new ItemStack(Item.DYE, 1, 0), new ItemStack(Item.DYE, 1, 15)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 7), [new ItemStack(Item.DYE, 1, 8), new ItemStack(Item.DYE, 1, 15)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 3, 7), [new ItemStack(Item.DYE, 1, 0), new ItemStack(Item.DYE, 1, 15), new ItemStack(Item.DYE, 1, 15)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 12), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 15)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 6), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 2)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 5), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 1)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 2, 13), [new ItemStack(Item.DYE, 1, 5), new ItemStack(Item.DYE, 1, 9)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 3, 13), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 9)]);
        var1.addShapelessRecipe(new ItemStack(Item.DYE, 4, 13), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 15)]);
    }
}