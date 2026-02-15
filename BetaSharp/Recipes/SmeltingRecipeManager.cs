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
        addSmelting(Block.IronOre.id, new ItemStack(Item.IRON_INGOT));
        addSmelting(Block.GoldOre.id, new ItemStack(Item.GOLD_INGOT));
        addSmelting(Block.DiamondOre.id, new ItemStack(Item.DIAMOND));
        addSmelting(Block.Sand.id, new ItemStack(Block.Glass));
        addSmelting(Item.RAW_PORKCHOP.id, new ItemStack(Item.COOKED_PORKCHOP));
        addSmelting(Item.RAW_FISH.id, new ItemStack(Item.COOKED_FISH));
        addSmelting(Block.Cobblestone.id, new ItemStack(Block.Stone));
        addSmelting(Item.CLAY.id, new ItemStack(Item.BRICK));
        addSmelting(Block.Cactus.id, new ItemStack(Item.DYE, 1, 2));
        addSmelting(Block.Log.id, new ItemStack(Item.COAL, 1, 1));
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