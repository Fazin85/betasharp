using betareborn.Blocks;

namespace betareborn.Textures
{
    public class TextureFlamesFX : TextureFX
    {

        protected float[] field_1133_g = new float[320];
        protected float[] field_1132_h = new float[320];

        public TextureFlamesFX(int var1) : base(Block.FIRE.textureId + var1 * 16)
        {
        }

        public override void onTick()
        {
            int var2;
            float var4;
            int var5;
            int var6;
            for (int var1 = 0; var1 < 16; ++var1)
            {
                for (var2 = 0; var2 < 20; ++var2)
                {
                    int var3 = 18;
                    var4 = field_1133_g[var1 + (var2 + 1) % 20 * 16] * (float)var3;

                    for (var5 = var1 - 1; var5 <= var1 + 1; ++var5)
                    {
                        for (var6 = var2; var6 <= var2 + 1; ++var6)
                        {
                            if (var5 >= 0 && var6 >= 0 && var5 < 16 && var6 < 20)
                            {
                                var4 += field_1133_g[var5 + var6 * 16];
                            }

                            ++var3;
                        }
                    }

                    field_1132_h[var1 + var2 * 16] = var4 / ((float)var3 * 1.06F);
                    if (var2 >= 19)
                    {
                        field_1132_h[var1 + var2 * 16] = (float)(java.lang.Math.random() * java.lang.Math.random() * java.lang.Math.random() * 4.0D + java.lang.Math.random() * (double)0.1F + (double)0.2F);
                    }
                }
            }

            float[] var12 = field_1132_h;
            field_1132_h = field_1133_g;
            field_1133_g = var12;

            for (var2 = 0; var2 < 256; ++var2)
            {
                float var13 = field_1133_g[var2] * 1.8F;
                if (var13 > 1.0F)
                {
                    var13 = 1.0F;
                }

                if (var13 < 0.0F)
                {
                    var13 = 0.0F;
                }

                var5 = (int)(var13 * 155.0F + 100.0F);
                var6 = (int)(var13 * var13 * 255.0F);
                int var7 = (int)(var13 * var13 * var13 * var13 * var13 * var13 * var13 * var13 * var13 * var13 * 255.0F);
                short var8 = 255;
                if (var13 < 0.5F)
                {
                    var8 = 0;
                }

                var4 = (var13 - 0.5F) * 2.0F;
                if (anaglyphEnabled)
                {
                    int var9 = (var5 * 30 + var6 * 59 + var7 * 11) / 100;
                    int var10 = (var5 * 30 + var6 * 70) / 100;
                    int var11 = (var5 * 30 + var7 * 70) / 100;
                    var5 = var9;
                    var6 = var10;
                    var7 = var11;
                }

                imageData[var2 * 4 + 0] = (byte)var5;
                imageData[var2 * 4 + 1] = (byte)var6;
                imageData[var2 * 4 + 2] = (byte)var7;
                imageData[var2 * 4 + 3] = (byte)var8;
            }

        }
    }

}