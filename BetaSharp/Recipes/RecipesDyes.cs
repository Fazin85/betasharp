using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesDyes
{
    public void AddRecipes(CraftingManager m)
    {
        for (int var2 = 0; var2 < 16; ++var2)
        {
            m.AddShapelessRecipe(new ItemStack(Block.Wool, 1, BlockCloth.getItemMeta(var2)), [new ItemStack(Item.DYE, 1, var2), new ItemStack(Item.ITEMS[Block.Wool.id], 1, 0)]);
        }

        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 11), new object[] { Block.Dandelion });
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 1), new object[] { Block.Rose });
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 3, 15), [Item.BONE]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 9), [new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 15)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 14), [new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 11)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 10), [new ItemStack(Item.DYE, 1, 2), new ItemStack(Item.DYE, 1, 15)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 8), [new ItemStack(Item.DYE, 1, 0), new ItemStack(Item.DYE, 1, 15)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 7), [new ItemStack(Item.DYE, 1, 8), new ItemStack(Item.DYE, 1, 15)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 3, 7), [new ItemStack(Item.DYE, 1, 0), new ItemStack(Item.DYE, 1, 15), new ItemStack(Item.DYE, 1, 15)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 12), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 15)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 6), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 2)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 5), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 1)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 2, 13), [new ItemStack(Item.DYE, 1, 5), new ItemStack(Item.DYE, 1, 9)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 3, 13), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 9)]);
        m.AddShapelessRecipe(new ItemStack(Item.DYE, 4, 13), [new ItemStack(Item.DYE, 1, 4), new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 1), new ItemStack(Item.DYE, 1, 15)]);
    }
}