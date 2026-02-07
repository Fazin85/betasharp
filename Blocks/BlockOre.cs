using betareborn.Items;
using betareborn.Materials;

namespace betareborn.Blocks
{
    public class BlockOre : Block
    {

        public BlockOre(int var1, int var2) : base(var1, var2, Material.STONE)
        {
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return id == Block.COAL_ORE.id ? Item.coal.id : (id == Block.DIAMOND_ORE.id ? Item.diamond.id : (id == Block.LAPIS_ORE.id ? Item.dyePowder.id : id));
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return id == Block.LAPIS_ORE.id ? 4 + var1.nextInt(5) : 1;
        }

        protected override int damageDropped(int var1)
        {
            return id == Block.LAPIS_ORE.id ? 4 : 0;
        }
    }

}