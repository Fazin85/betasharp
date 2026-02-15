using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesFood
{
    public void AddRecipes(CraftingManager m)
    {
        m.AddRecipe(new ItemStack(Item.MUSHROOM_STEW), new object[] { "Y", "X", "#", 'X', Block.BrownMushroom, 'Y', Block.RedMushroom, '#', Item.BOWL });
        m.AddRecipe(new ItemStack(Item.MUSHROOM_STEW), new object[] { "Y", "X", "#", 'X', Block.RedMushroom, 'Y', Block.BrownMushroom, '#', Item.BOWL });
        m.AddRecipe(new ItemStack(Item.COOKIE, 8), ["#X#", 'X', new ItemStack(Item.DYE, 1, 3), '#', Item.WHEAT]);
    }
}