using betareborn.Blocks;
using betareborn.Items;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntitySlimeFX : EntityFX
    {

        public EntitySlimeFX(World var1, double var2, double var4, double var6, Item var8) : base(var1, var2, var4, var6, 0.0D, 0.0D, 0.0D)
        {
            particleTextureIndex = var8.getIconFromDamage(0);
            particleRed = particleGreen = particleBlue = 1.0F;
            particleGravity = Block.SNOW_BLOCK.particleFallSpeedModifier;
            particleScale /= 2.0F;
        }

        public override int getFXLayer()
        {
            return 2;
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