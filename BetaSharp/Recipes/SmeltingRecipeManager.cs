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
        AddSmelting(Block.IronOre.id, new ItemStack(Item.IRON_INGOT));
        AddSmelting(Block.GoldOre.id, new ItemStack(Item.GOLD_INGOT));
        AddSmelting(Block.DiamondOre.id, new ItemStack(Item.DIAMOND));
        AddSmelting(Block.Sand.id, new ItemStack(Block.Glass));
        AddSmelting(Item.RAW_PORKCHOP.id, new ItemStack(Item.COOKED_PORKCHOP));
        AddSmelting(Item.RAW_FISH.id, new ItemStack(Item.COOKED_FISH));
        AddSmelting(Block.Cobblestone.id, new ItemStack(Block.Stone));
        AddSmelting(Item.CLAY.id, new ItemStack(Item.BRICK));
        AddSmelting(Block.Cactus.id, new ItemStack(Item.DYE, 1, 2));
        AddSmelting(Block.Log.id, new ItemStack(Item.COAL, 1, 1));
    }

    public void AddSmelting(int inputId, ItemStack output)
    {
        smeltingList[inputId] = output;
    }

    public ItemStack Craft(int inputId)
    {
        return smeltingList[inputId];
    }

    public Dictionary<int, ItemStack> GetSmeltingList()
    {
        return smeltingList;
    }
}