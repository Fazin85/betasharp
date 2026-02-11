using betareborn.Client.Rendering.Core;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityFX : Entity
    {

        protected int particleTextureIndex;
        protected float particleTextureJitterX;
        protected float particleTextureJitterY;
        protected int particleAge = 0;
        protected int particleMaxAge = 0;
        protected float particleScale;
        protected float particleGravity;
        protected float particleRed;
        protected float particleGreen;
        protected float particleBlue;
        public static double interpPosX;
        public static double interpPosY;
        public static double interpPosZ;

        public EntityFX(World var1, double var2, double var4, double var6, double var8, double var10, double var12) : base(var1)
        {
            setBoundingBoxSpacing(0.2F, 0.2F);
            standingEyeHeight = height / 2.0F;
            setPosition(var2, var4, var6);
            particleRed = particleGreen = particleBlue = 1.0F;
            velocityX = var8 + (double)((float)(java.lang.Math.random() * 2.0D - 1.0D) * 0.4F);
            velocityY = var10 + (double)((float)(java.lang.Math.random() * 2.0D - 1.0D) * 0.4F);
            velocityZ = var12 + (double)((float)(java.lang.Math.random() * 2.0D - 1.0D) * 0.4F);
            float var14 = (float)(java.lang.Math.random() + java.lang.Math.random() + 1.0D) * 0.15F;
            float var15 = MathHelper.sqrt_double(velocityX * velocityX + velocityY * velocityY + velocityZ * velocityZ);
            velocityX = velocityX / (double)var15 * (double)var14 * (double)0.4F;
            velocityY = velocityY / (double)var15 * (double)var14 * (double)0.4F + (double)0.1F;
            velocityZ = velocityZ / (double)var15 * (double)var14 * (double)0.4F;
            particleTextureJitterX = random.nextFloat() * 3.0F;
            particleTextureJitterY = random.nextFloat() * 3.0F;
            particleScale = (random.nextFloat() * 0.5F + 0.5F) * 2.0F;
            particleMaxAge = (int)(4.0F / (random.nextFloat() * 0.9F + 0.1F));
            particleAge = 0;
        }

        public EntityFX func_407_b(float var1)
        {
            velocityX *= (double)var1;
            velocityY = (velocityY - (double)0.1F) * (double)var1 + (double)0.1F;
            velocityZ *= (double)var1;
            return this;
        }

        public EntityFX func_405_d(float var1)
        {
            setBoundingBoxSpacing(0.2F * var1, 0.2F * var1);
            particleScale *= var1;
            return this;
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        protected override void initDataTracker()
        {
        }

        public override void tick()
        {
            prevX = x;
            prevY = y;
            prevZ = z;
            if (particleAge++ >= particleMaxAge)
            {
                markDead();
            }

            velocityY -= 0.04D * (double)particleGravity;
            move(velocityX, velocityY, velocityZ);
            velocityX *= (double)0.98F;
            velocityY *= (double)0.98F;
            velocityZ *= (double)0.98F;
            if (onGround)
            {
                velocityX *= (double)0.7F;
                velocityZ *= (double)0.7F;
            }

        }

        public virtual void renderParticle(Tessellator var1, float var2, float var3, float var4, float var5, float var6, float var7)
        {
            float var8 = (float)(particleTextureIndex % 16) / 16.0F;
            float var9 = var8 + 0.999F / 16.0F;
            float var10 = (float)(particleTextureIndex / 16) / 16.0F;
            float var11 = var10 + 0.999F / 16.0F;
            float var12 = 0.1F * particleScale;
            float var13 = (float)(prevX + (x - prevX) * (double)var2 - interpPosX);
            float var14 = (float)(prevY + (y - prevY) * (double)var2 - interpPosY);
            float var15 = (float)(prevZ + (z - prevZ) * (double)var2 - interpPosZ);
            float var16 = getBrightnessAtEyes(var2);
            var1.setColorOpaque_F(particleRed * var16, particleGreen * var16, particleBlue * var16);
            var1.addVertexWithUV((double)(var13 - var3 * var12 - var6 * var12), (double)(var14 - var4 * var12), (double)(var15 - var5 * var12 - var7 * var12), (double)var9, (double)var11);
            var1.addVertexWithUV((double)(var13 - var3 * var12 + var6 * var12), (double)(var14 + var4 * var12), (double)(var15 - var5 * var12 + var7 * var12), (double)var9, (double)var10);
            var1.addVertexWithUV((double)(var13 + var3 * var12 + var6 * var12), (double)(var14 + var4 * var12), (double)(var15 + var5 * var12 + var7 * var12), (double)var8, (double)var10);
            var1.addVertexWithUV((double)(var13 + var3 * var12 - var6 * var12), (double)(var14 - var4 * var12), (double)(var15 + var5 * var12 - var7 * var12), (double)var8, (double)var11);
        }

        public virtual int getFXLayer()
        {
            return 0;
        }

        public override void writeNbt(NbtTagCompound var1)
        {
        }

        public override void readNbt(NbtTagCompound var1)
        {
        }
    }

}