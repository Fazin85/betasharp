using betareborn.Inventorys;
using betareborn.Items;

namespace betareborn.Recipes
{
    public interface IRecipe
    {
        bool matches(InventoryCrafting var1);

        ItemStack getCraftingResult(InventoryCrafting var1);

        int getRecipeSize();

        ItemStack getRecipeOutput();
    }

}