using betareborn.Blocks;
using java.util;

namespace betareborn
{
    public class Session
    {
        public static List registeredBlocksList = new ArrayList();
        public string username;
        public string sessionId;
        public string mpPassParameter;

        public Session(string var1, string var2)
        {
            username = var1;
            sessionId = var2;
        }

        static Session()
        {
            registeredBlocksList.add(Block.STONE);
            registeredBlocksList.add(Block.COBBLESTONE);
            registeredBlocksList.add(Block.BRICKS);
            registeredBlocksList.add(Block.DIRT);
            registeredBlocksList.add(Block.PLANKS);
            registeredBlocksList.add(Block.LOG);
            registeredBlocksList.add(Block.LEAVES);
            registeredBlocksList.add(Block.TORCH);
            registeredBlocksList.add(Block.SLAB);
            registeredBlocksList.add(Block.GLASS);
            registeredBlocksList.add(Block.MOSSY_COBBLESTONE);
            registeredBlocksList.add(Block.SAPLING);
            registeredBlocksList.add(Block.DANDELION);
            registeredBlocksList.add(Block.ROSE);
            registeredBlocksList.add(Block.BROWN_MUSHROOM);
            registeredBlocksList.add(Block.RED_MUSHROOM);
            registeredBlocksList.add(Block.SAND);
            registeredBlocksList.add(Block.GRAVEL);
            registeredBlocksList.add(Block.SPONGE);
            registeredBlocksList.add(Block.WOOL);
            registeredBlocksList.add(Block.COAL_ORE);
            registeredBlocksList.add(Block.IRON_ORE);
            registeredBlocksList.add(Block.GOLD_ORE);
            registeredBlocksList.add(Block.IRON_BLOCK);
            registeredBlocksList.add(Block.GOLD_BLOCK);
            registeredBlocksList.add(Block.BOOKSHELF);
            registeredBlocksList.add(Block.TNT);
            registeredBlocksList.add(Block.OBSIDIAN);
        }
    }

}