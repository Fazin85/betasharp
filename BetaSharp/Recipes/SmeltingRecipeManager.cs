using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;
using java.util;

namespace BetaSharp.Recipes;

public class SmeltingRecipeManager
{
    private static readonly SmeltingRecipeManager smeltingBase = new();
    private Map smeltingList = new HashMap();

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

    public void addSmelting(int var1, ItemStack var2)
    {
        smeltingList.put(Integer.valueOf(var1), var2);
    }

    public ItemStack craft(int var1)
    {
        return (ItemStack)smeltingList.get(Integer.valueOf(var1));
    }

    public Map getSmeltingList()
    {
        return smeltingList;
    }
}