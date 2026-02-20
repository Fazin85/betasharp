using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class SmeltingRecipeManager
{
    private static readonly SmeltingRecipeManager smeltingBase = new();
    private Dictionary<int, ItemStack> smeltingList = new();

    public static SmeltingRecipeManager getInstance()
    {
        return smeltingBase;
    }

    private SmeltingRecipeManager()
    {
        RecipeLoader.LoadSmelting(this, "Assets/Recipes/smelting.json");
        Log.Info($"[SmeltingRecipeManager] {smeltingList.Count} smelting recipes loaded from data.");
    }

    public void AddSmelting(int inputId, ItemStack output)
    {
        smeltingList[inputId] = output;
    }

    public ItemStack? Craft(int inputId)
    {
        if (smeltingList.TryGetValue(inputId, out ItemStack? result))
        {
            return result;
        }
        return null;
    }

    public Dictionary<int, ItemStack> GetSmeltingList()
    {
        return smeltingList;
    }
}
