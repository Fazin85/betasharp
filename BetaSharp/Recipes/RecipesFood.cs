using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class RecipesFood
{
    public void AddRecipes(CraftingManager m)
    {
        var1.addRecipe(new ItemStack(Item.MUSHROOM_STEW), new object[] { "Y", "X", "#", Character.valueOf('X'), Block.BrownMushroom, Character.valueOf('Y'), Block.RedMushroom, Character.valueOf('#'), Item.BOWL });
        var1.addRecipe(new ItemStack(Item.MUSHROOM_STEW), new object[] { "Y", "X", "#", Character.valueOf('X'), Block.RedMushroom, Character.valueOf('Y'), Block.BrownMushroom, Character.valueOf('#'), Item.BOWL });
        var1.addRecipe(new ItemStack(Item.COOKIE, 8), ["#X#", Character.valueOf('X'), new ItemStack(Item.DYE, 1, 3), Character.valueOf('#'), Item.WHEAT]);
    }
}