using betareborn.Blocks;

namespace betareborn.Items
{

    public class ItemAxe : ItemTool
    {

        private static Block[] blocksEffectiveAgainst = new Block[] { Block.PLANKS, Block.BOOKSHELF, Block.LOG, Block.CHEST };

        public ItemAxe(int var1, EnumToolMaterial var2) : base(var1, 3, var2, blocksEffectiveAgainst)
        {
        }
    }

}