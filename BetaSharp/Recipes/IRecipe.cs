using BetaSharp.Inventorys;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public interface IRecipe
{
    bool matches(InventoryCrafting var1);

    ItemStack getCraftingResult(InventoryCrafting var1);

    int getRecipeSize();

    ItemStack getRecipeOutput();
}