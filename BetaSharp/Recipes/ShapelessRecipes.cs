using BetaSharp.Inventorys;
using BetaSharp.Items;
using java.util;

namespace BetaSharp.Recipes;

public class ShapelessRecipes : IRecipe
{

    private readonly ItemStack recipeOutput;
    private readonly List recipeItems;

    public ShapelessRecipes(ItemStack var1, List var2)
    {
        recipeOutput = var1;
        recipeItems = var2;
    }

    public ItemStack getRecipeOutput()
    {
        return recipeOutput;
    }

    public bool matches(InventoryCrafting var1)
    {
        ArrayList var2 = new ArrayList(recipeItems);

        for (int var3 = 0; var3 < 3; ++var3)
        {
            for (int var4 = 0; var4 < 3; ++var4)
            {
                ItemStack var5 = var1.getStackAt(var4, var3);
                if (var5 != null)
                {
                    bool var6 = false;
                    Iterator var7 = var2.iterator();

                    while (var7.hasNext())
                    {
                        ItemStack var8 = (ItemStack)var7.next();
                        if (var5.itemId == var8.itemId && (var8.getDamage() == -1 || var5.getDamage() == var8.getDamage()))
                        {
                            var6 = true;
                            var2.remove(var8);
                            break;
                        }
                    }

                    if (!var6)
                    {
                        return false;
                    }
                }
            }
        }

        return var2.isEmpty();
    }

    public ItemStack getCraftingResult(InventoryCrafting var1)
    {
        return recipeOutput.copy();
    }

    public int getRecipeSize()
    {
        return recipeItems.size();
    }
}