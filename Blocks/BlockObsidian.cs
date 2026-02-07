namespace betareborn.Blocks
{
    public class BlockObsidian : BlockStone
    {
        public BlockObsidian(int var1, int var2) : base(var1, var2)
        {
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 1;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.OBSIDIAN.id;
        }
    }

}