using betareborn.Client.Rendering.Core;
using betareborn.Util.Maths;
using betareborn.Worlds;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Entities
{
    public class EntityFootStepFX : EntityFX
    {

        private int field_27018_a = 0;
        private int field_27020_o = 0;
        private TextureManager field_27019_p;

        public EntityFootStepFX(TextureManager var1, World var2, double var3, double var5, double var7) : base(var2, var3, var5, var7, 0.0D, 0.0D, 0.0D)
        {
            field_27019_p = var1;
            velocityX = velocityY = velocityZ = 0.0D;
            field_27020_o = 200;
        }

        public override void renderParticle(Tessellator var1, float var2, float var3, float var4, float var5, float var6, float var7)
        {
            float var8 = ((float)field_27018_a + var2) / (float)field_27020_o;
            var8 *= var8;
            float var9 = 2.0F - var8 * 2.0F;
            if (var9 > 1.0F)
            {
                var9 = 1.0F;
            }

            var9 *= 0.2F;
            GLManager.GL.Disable(GLEnum.Lighting);
            float var10 = 2.0F / 16.0F;
            float var11 = (float)(x - interpPosX);
            float var12 = (float)(y - interpPosY);
            float var13 = (float)(z - interpPosZ);
            float var14 = world.GetLuminance(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z));
            field_27019_p.bindTexture(field_27019_p.getTextureId("/misc/footprint.png"));
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            var1.startDrawingQuads();
            var1.setColorRGBA_F(var14, var14, var14, var9);
            var1.addVertexWithUV((double)(var11 - var10), (double)var12, (double)(var13 + var10), 0.0D, 1.0D);
            var1.addVertexWithUV((double)(var11 + var10), (double)var12, (double)(var13 + var10), 1.0D, 1.0D);
            var1.addVertexWithUV((double)(var11 + var10), (double)var12, (double)(var13 - var10), 1.0D, 0.0D);
            var1.addVertexWithUV((double)(var11 - var10), (double)var12, (double)(var13 - var10), 0.0D, 0.0D);
            var1.draw();
            GLManager.GL.Disable(GLEnum.Blend);
            GLManager.GL.Enable(GLEnum.Lighting);
        }

        public override void tick()
        {
            ++field_27018_a;
            if (field_27018_a == field_27020_o)
            {
                markDead();
            }

        }

        public override int getFXLayer()
        {
            return 3;
        }
    }

}