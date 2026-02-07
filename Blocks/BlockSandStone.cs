using betareborn.Materials;

namespace betareborn.Blocks
{
    public class BlockSandStone : Block
    {
        public BlockSandStone(int var1) : base(var1, 192, Material.STONE)
        {
        }

        public override int getTexture(int var1)
        {
            return var1 == 1 ? textureId - 16 : (var1 == 0 ? textureId + 16 : textureId);
        }
    }

}