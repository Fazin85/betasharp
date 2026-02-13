using BetaSharp.Inventorys;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class ShapedRecipes : java.lang.Object, IRecipe
{
    private int recipeWidth;
    private int recipeHeight;
    private ItemStack[] recipeItems;
    private ItemStack recipeOutput;
    public readonly int recipeOutputItemID;

    public ShapedRecipes(int var1, int var2, ItemStack[] var3, ItemStack var4)
    {
        recipeOutputItemID = var4.itemId;
        recipeWidth = var1;
        recipeHeight = var2;
        recipeItems = var3;
        recipeOutput = var4;
    }

    public ItemStack getRecipeOutput()
    {
        return recipeOutput;
    }

    public bool matches(InventoryCrafting var1)
    {
        for (int var2 = 0; var2 <= 3 - recipeWidth; ++var2)
        {
            for (int var3 = 0; var3 <= 3 - recipeHeight; ++var3)
            {
                if (func_21137_a(var1, var2, var3, true))
                {
                    return true;
                }

                if (func_21137_a(var1, var2, var3, false))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool func_21137_a(InventoryCrafting var1, int var2, int var3, bool var4)
    {
        for (int var5 = 0; var5 < 3; ++var5)
        {
            for (int var6 = 0; var6 < 3; ++var6)
            {
                int var7 = var5 - var2;
                int var8 = var6 - var3;
                ItemStack var9 = null;
                if (var7 >= 0 && var8 >= 0 && var7 < recipeWidth && var8 < recipeHeight)
                {
                    if (var4)
                    {
                        var9 = recipeItems[recipeWidth - var7 - 1 + var8 * recipeWidth];
                    }
                    else
                    {
                        var9 = recipeItems[var7 + var8 * recipeWidth];
                    }
                }

                ItemStack var10 = var1.getStackAt(var5, var6);
                if (var10 != null || var9 != null)
                {
                    if (var10 == null && var9 != null || var10 != null && var9 == null)
                    {
                        return false;
                    }

                    if (var9.itemId != var10.itemId)
                    {
                        return false;
                    }

                    if (var9.getDamage() != -1 && var9.getDamage() != var10.getDamage())
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public ItemStack getCraftingResult(InventoryCrafting var1)
    {
        return new ItemStack(recipeOutput.itemId, recipeOutput.count, recipeOutput.getDamage());
    }

    public int getRecipeSize()
    {
        return recipeWidth * recipeHeight;
    }
}