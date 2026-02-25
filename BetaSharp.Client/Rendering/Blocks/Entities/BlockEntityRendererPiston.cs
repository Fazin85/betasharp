using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Blocks.Entities;

public class BlockEntityRendererPiston : BlockEntitySpecialRenderer
{

    private BlockRenderer renderBlocks;

    public void func_31070_a(BlockEntityPiston var1, double var2, double var4, double var6, float var8)
    {
        Block var9 = Block.Blocks[var1.getPushedBlockId()];
        if (var9 != null && var1.getProgress(var8) < 1.0F)
        {
            Tessellator var10 = Tessellator.instance;
            bindTextureByName("/terrain.png");
            Lighting.turnOff();
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.Disable(GLEnum.CullFace);
            if (Minecraft.isAmbientOcclusionEnabled())
            {
                GLManager.GL.ShadeModel(GLEnum.Smooth);
            }
            else
            {
                GLManager.GL.ShadeModel(GLEnum.Flat);
            }

            var10.startDrawingQuads();
            var10.setTranslationD((double)((float)var2 - var1.X + var1.getRenderOffsetX(var8)), (double)((float)var4 - var1.Y + var1.getRenderOffsetY(var8)), (double)((float)var6 - var1.Z + var1.getRenderOffsetZ(var8)));
            var10.setColorOpaque(1, 1, 1);
            if (var9 == Block.PistonHead && var1.getProgress(var8) < 0.5F)
            {
                renderBlocks.func_31079_a(var9, var1.X, var1.Y, var1.Z, false);
            }
            else if (var1.isSource() && !var1.isExtending())
            {
                Block.PistonHead.setSprite(((BlockPistonBase)var9).getTopTexture());
                renderBlocks.func_31079_a(Block.PistonHead, var1.X, var1.Y, var1.Z, var1.getProgress(var8) < 0.5F);
                Block.PistonHead.clearSprite();
                var10.setTranslationD((double)((float)var2 - var1.X), (double)((float)var4 - var1.Y), (double)((float)var6 - var1.Z));
                renderBlocks.func_31078_d(var9, var1.X, var1.Y, var1.Z);
            }
            else
            {
                renderBlocks.func_31075_a(var9, var1.X, var1.Y, var1.Z);
            }

            var10.setTranslationD(0.0D, 0.0D, 0.0D);
            var10.draw();
            Lighting.turnOn();
        }

    }

    public override void func_31069_a(World var1)
    {
        renderBlocks = new BlockRenderer(var1, Tessellator.instance);
    }

    public override void renderTileEntityAt(BlockEntity var1, double var2, double var4, double var6, float var8)
    {
        func_31070_a((BlockEntityPiston)var1, var2, var4, var6, var8);
    }
}