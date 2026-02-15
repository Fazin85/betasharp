using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesFood
{
    public void AddRecipes(CraftingManager m)
    {
        m.AddRecipe(new ItemStack(Item.MUSHROOM_STEW), [ "Y", "X", "#", 'X', Block.BROWN_MUSHROOM, 'Y', Block.RED_MUSHROOM, '#', Item.BOWL ]);
        m.AddRecipe(new ItemStack(Item.MUSHROOM_STEW), [ "Y", "X", "#", 'X', Block.RED_MUSHROOM, 'Y', Block.BROWN_MUSHROOM, '#', Item.BOWL ]);
        m.AddRecipe(new ItemStack(Item.COOKIE, 8), ["#X#", 'X', new ItemStack(Item.DYE, 1, 3), '#', Item.WHEAT]);
    }
}