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
        AddSmelting(Block.IRON_ORE.id, new ItemStack(Item.IRON_INGOT));
        AddSmelting(Block.GOLD_ORE.id, new ItemStack(Item.GOLD_INGOT));
        AddSmelting(Block.DIAMOND_ORE.id, new ItemStack(Item.DIAMOND));
        AddSmelting(Block.SAND.id, new ItemStack(Block.GLASS));
        AddSmelting(Item.RAW_PORKCHOP.id, new ItemStack(Item.COOKED_PORKCHOP));
        AddSmelting(Item.RAW_FISH.id, new ItemStack(Item.COOKED_FISH));
        AddSmelting(Block.COBBLESTONE.id, new ItemStack(Block.STONE));
        AddSmelting(Item.CLAY.id, new ItemStack(Item.BRICK));
        AddSmelting(Block.CACTUS.id, new ItemStack(Item.DYE, 1, 2));
        AddSmelting(Block.LOG.id, new ItemStack(Item.COAL, 1, 1));
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