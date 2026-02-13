using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesFood
{
    public void addRecipes(CraftingManager var1)
    {
        var1.addRecipe(new ItemStack(Item.MUSHROOM_STEW), new object[] { "Y", "X", "#", Character.valueOf('X'), Block.BROWN_MUSHROOM, Character.valueOf('Y'), Block.RED_MUSHROOM, Character.valueOf('#'), Item.BOWL });
        var1.addRecipe(new ItemStack(Item.MUSHROOM_STEW), new object[] { "Y", "X", "#", Character.valueOf('X'), Block.RED_MUSHROOM, Character.valueOf('Y'), Block.BROWN_MUSHROOM, Character.valueOf('#'), Item.BOWL });
        var1.addRecipe(new ItemStack(Item.COOKIE, 8), ["#X#", Character.valueOf('X'), new ItemStack(Item.DYE, 1, 3), Character.valueOf('#'), Item.WHEAT]);
    }
}