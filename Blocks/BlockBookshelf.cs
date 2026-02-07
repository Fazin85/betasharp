using betareborn.Materials;

namespace betareborn.Blocks
{
    public class BlockBookshelf : Block
    {
        public BlockBookshelf(int var1, int var2) : base(var1, var2, Material.WOOD)
        {
        }

        public override int getTexture(int var1)
        {
            return var1 <= 1 ? 4 : textureId;
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 0;
        }
    }

}