using BetaSharp.Inventorys;
using BetaSharp.Items;
using java.util;

namespace BetaSharp.Recipes;

public class ShapelessRecipes : IRecipe
{

    private readonly ItemStack _output;
    private readonly List _recipeItems;

    public ShapelessRecipes(ItemStack output, List items)
    {
        _output = output;
        _recipeItems = items;
    }

    public ItemStack GetRecipeOutput()
    {
        return _output;
    }

    public bool Matches(InventoryCrafting craftingInventory)
    {
        ArrayList remainingIngredients = new ArrayList(_recipeItems);

        for (int row = 0; row < 3; ++row)
        {
            for (int col = 0; col < 3; ++col)
            {
                ItemStack gridStack = craftingInventory.getStackAt(col, row);
                if (gridStack != null)
                {
                    bool foundMatch = false;
                    Iterator iterator = remainingIngredients.iterator();

                    while (iterator.hasNext())
                    {
                        ItemStack recipeItem = (ItemStack)iterator.next();
                        if (gridStack.itemId == recipeItem.itemId && (recipeItem.getDamage() == -1 || gridStack.getDamage() == recipeItem.getDamage()))
                        {
                            foundMatch = true;
                            remainingIngredients.remove(recipeItem);
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        return false;
                    }
                }
            }
        }

        return remainingIngredients.isEmpty();
    }

    public ItemStack GetCraftingResult(InventoryCrafting craftingInventory)
    {
        return _output.copy();
    }

    public int GetRecipeSize()
    {
        return _recipeItems.size();
    }
}