using betareborn.Blocks;
using betareborn.Items;
using java.lang;
using java.util;

namespace betareborn.Recipes
{
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
            addSmelting(Block.IRON_ORE.id, new ItemStack(Item.ingotIron));
            addSmelting(Block.GOLD_ORE.id, new ItemStack(Item.ingotGold));
            addSmelting(Block.DIAMOND_ORE.id, new ItemStack(Item.diamond));
            addSmelting(Block.SAND.id, new ItemStack(Block.GLASS));
            addSmelting(Item.porkRaw.id, new ItemStack(Item.porkCooked));
            addSmelting(Item.fishRaw.id, new ItemStack(Item.fishCooked));
            addSmelting(Block.COBBLESTONE.id, new ItemStack(Block.STONE));
            addSmelting(Item.clay.id, new ItemStack(Item.brick));
            addSmelting(Block.CACTUS.id, new ItemStack(Item.dyePowder, 1, 2));
            addSmelting(Block.LOG.id, new ItemStack(Item.coal, 1, 1));
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

}