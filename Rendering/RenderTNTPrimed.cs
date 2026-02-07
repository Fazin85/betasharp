using betareborn.Blocks;
using betareborn.Entities;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Rendering
{
    public class RenderTNTPrimed : Render
    {

        private RenderBlocks blockRenderer = new RenderBlocks();

        public RenderTNTPrimed()
        {
            shadowSize = 0.5F;
        }

        public void func_153_a(EntityTNTPrimed var1, double var2, double var4, double var6, float var8, float var9)
        {
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
            float var10;
            if ((float)var1.fuse - var9 + 1.0F < 10.0F)
            {
                var10 = 1.0F - ((float)var1.fuse - var9 + 1.0F) / 10.0F;
                if (var10 < 0.0F)
                {
                    var10 = 0.0F;
                }

                if (var10 > 1.0F)
                {
                    var10 = 1.0F;
                }

                var10 *= var10;
                var10 *= var10;
                float var11 = 1.0F + var10 * 0.3F;
                GLManager.GL.Scale(var11, var11, var11);
            }

            var10 = (1.0F - ((float)var1.fuse - var9 + 1.0F) / 100.0F) * 0.8F;
            loadTexture("/terrain.png");
            blockRenderer.renderBlockOnInventory(Block.TNT, 0, var1.getEntityBrightness(var9));
            if (var1.fuse / 5 % 2 == 0)
            {
                GLManager.GL.Disable(GLEnum.Texture2D);
                GLManager.GL.Disable(GLEnum.Lighting);
                GLManager.GL.Enable(GLEnum.Blend);
                GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.DstAlpha);
                GLManager.GL.Color4(1.0F, 1.0F, 1.0F, var10);
                blockRenderer.renderBlockOnInventory(Block.TNT, 0, 1.0F);
                GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
                GLManager.GL.Disable(GLEnum.Blend);
                GLManager.GL.Enable(GLEnum.Lighting);
                GLManager.GL.Enable(GLEnum.Texture2D);
            }
            GLManager.GL.PopMatrix();
        }

        public override void doRender(Entity var1, double var2, double var4, double var6, float var8, float var9)
        {
            func_153_a((EntityTNTPrimed)var1, var2, var4, var6, var8, var9);
        }
    }

}