using betareborn.Blocks.Materials;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityBubbleFX : EntityFX
    {

        public EntityBubbleFX(World var1, double var2, double var4, double var6, double var8, double var10, double var12) : base(var1, var2, var4, var6, var8, var10, var12)
        {
            particleRed = 1.0F;
            particleGreen = 1.0F;
            particleBlue = 1.0F;
            particleTextureIndex = 32;
            setBoundingBoxSpacing(0.02F, 0.02F);
            particleScale *= random.nextFloat() * 0.6F + 0.2F;
            velocityX = var8 * (double)0.2F + (double)((float)(java.lang.Math.random() * 2.0D - 1.0D) * 0.02F);
            velocityY = var10 * (double)0.2F + (double)((float)(java.lang.Math.random() * 2.0D - 1.0D) * 0.02F);
            velocityZ = var12 * (double)0.2F + (double)((float)(java.lang.Math.random() * 2.0D - 1.0D) * 0.02F);
            particleMaxAge = (int)(8.0D / (java.lang.Math.random() * 0.8D + 0.2D));
        }

        public override void tick()
        {
            prevX = x;
            prevY = y;
            prevZ = z;
            velocityY += 0.002D;
            move(velocityX, velocityY, velocityZ);
            velocityX *= (double)0.85F;
            velocityY *= (double)0.85F;
            velocityZ *= (double)0.85F;
            if (world.GetMaterial(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z)) != Material.WATER)
            {
                markDead();
            }

            if (particleMaxAge-- <= 0)
            {
                markDead();
            }

        }
    }

}