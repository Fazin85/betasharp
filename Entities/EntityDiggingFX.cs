using betareborn.Blocks;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityDiggingFX : EntityFX
    {

        private Block field_4082_a;
        private int field_32001_o = 0;

        public EntityDiggingFX(World var1, double var2, double var4, double var6, double var8, double var10, double var12, Block var14, int var15, int var16) : base(var1, var2, var4, var6, var8, var10, var12)
        {
            field_4082_a = var14;
            particleTextureIndex = var14.getTexture(0, var16);
            particleGravity = var14.particleFallSpeedModifier;
            particleRed = particleGreen = particleBlue = 0.6F;
            particleScale /= 2.0F;
            field_32001_o = var15;
        }

        public EntityDiggingFX func_4041_a(int var1, int var2, int var3)
        {
            if (field_4082_a == Block.GRASS_BLOCK)
            {
                return this;
            }
            else
            {
                int var4 = field_4082_a.colorMultiplier(worldObj, var1, var2, var3);
                particleRed *= (float)(var4 >> 16 & 255) / 255.0F;
                particleGreen *= (float)(var4 >> 8 & 255) / 255.0F;
                particleBlue *= (float)(var4 & 255) / 255.0F;
                return this;
            }
        }

        public override int getFXLayer()
        {
            return 1;
        }

        public override void renderParticle(Tessellator var1, float var2, float var3, float var4, float var5, float var6, float var7)
        {
            float var8 = ((float)(particleTextureIndex % 16) + particleTextureJitterX / 4.0F) / 16.0F;
            float var9 = var8 + 0.999F / 64.0F;
            float var10 = ((float)(particleTextureIndex / 16) + particleTextureJitterY / 4.0F) / 16.0F;
            float var11 = var10 + 0.999F / 64.0F;
            float var12 = 0.1F * particleScale;
            float var13 = (float)(prevPosX + (posX - prevPosX) * (double)var2 - interpPosX);
            float var14 = (float)(prevPosY + (posY - prevPosY) * (double)var2 - interpPosY);
            float var15 = (float)(prevPosZ + (posZ - prevPosZ) * (double)var2 - interpPosZ);
            float var16 = getEntityBrightness(var2);
            var1.setColorOpaque_F(var16 * particleRed, var16 * particleGreen, var16 * particleBlue);
            var1.addVertexWithUV((double)(var13 - var3 * var12 - var6 * var12), (double)(var14 - var4 * var12), (double)(var15 - var5 * var12 - var7 * var12), (double)var8, (double)var11);
            var1.addVertexWithUV((double)(var13 - var3 * var12 + var6 * var12), (double)(var14 + var4 * var12), (double)(var15 - var5 * var12 + var7 * var12), (double)var8, (double)var10);
            var1.addVertexWithUV((double)(var13 + var3 * var12 + var6 * var12), (double)(var14 + var4 * var12), (double)(var15 + var5 * var12 + var7 * var12), (double)var9, (double)var10);
            var1.addVertexWithUV((double)(var13 + var3 * var12 - var6 * var12), (double)(var14 - var4 * var12), (double)(var15 + var5 * var12 - var7 * var12), (double)var9, (double)var11);
        }
    }

}