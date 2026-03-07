using BetaSharp.Items;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Recipes;

public class SmeltingRecipeManager
{
    private static readonly SmeltingRecipeManager smeltingBase = new();
    private readonly Dictionary<int, ItemStack> smeltingList = [];
    private readonly ILogger _logger = Log.Instance.For<SmeltingRecipeManager>();

    public static SmeltingRecipeManager getInstance()
    {
        return smeltingBase;
    }

    private SmeltingRecipeManager()
    {
        RecipeLoader.LoadSmelting(this, "Assets/Recipes/smelting.json");
        _logger.LogInformation($"{smeltingList.Count} smelting recipes loaded from data.");
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
