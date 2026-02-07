using betareborn.Materials;

namespace betareborn.Blocks
{
    public class BlockOreStorage : Block
    {

        public BlockOreStorage(int var1, int var2) : base(var1, Material.METAL)
        {
            textureId = var2;
        }

        public override int getTexture(int var1)
        {
            return textureId;
        }
    }

}