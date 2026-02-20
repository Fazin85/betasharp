using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entities;

public abstract class EntityRenderer
{
    protected EntityRenderDispatcher dispatcher;
    protected float shadowRadius = 0.0F;
    protected float shadowStrength = 1.0F;

    public abstract void render(Entity target, double x, double y, double z, float yaw, float tickDelta);

    protected void loadTexture(string path)
    {
        TextureManager? var2 = dispatcher.textureManager;
        var2?.BindTexture(var2.GetTextureId(path));
    }

    protected bool loadDownloadableImageTexture(string var1, string var2)
    {
        //RenderEngine var3 = renderManager.renderEngine;
        if (var2 == null)
        {
            return false;
        }

        loadTexture(var2);
        return true;
        //TODO: CUSTOM SKINS
        //int var4 = var3.getTextureForDownloadableImage(var1, var2);
        //int var4 = var3.getTexture(var1);
        //if (var4 >= 0)
        //{
        //    var3.bindTexture(var4);
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        return false;
    }

    private void renderOnFire(Entity var1, double var2, double var4, double var6, float var8)
    {
        GLManager.GL.Disable(GLEnum.Lighting);
        int var9 = Block.Fire.textureId;
        int var10 = (var9 & 15) << 4;
        int var11 = var9 & 240;
        float var12 = var10 / 256.0F;
        float var13 = (var10 + 15.99F) / 256.0F;
        float var14 = var11 / 256.0F;
        float var15 = (var11 + 15.99F) / 256.0F;
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
        float var16 = var1.width * 1.4F;
        GLManager.GL.Scale(var16, var16, var16);
        loadTexture("/terrain.png");
        Tessellator var17 = Tessellator.instance;
        float var18 = 0.5F;
        float var19 = 0.0F;
        float var20 = var1.height / var16;
        float var21 = (float)(var1.y - var1.boundingBox.minY);
        GLManager.GL.Rotate(-dispatcher.playerViewY, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Translate(0.0F, 0.0F, -0.3F + (int)var20 * 0.02F);
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        float var22 = 0.0F;
        int var23 = 0;
        var17.startDrawingQuads();

        while (var20 > 0.0F)
        {
            if (var23 % 2 == 0)
            {
                var12 = var10 / 256.0F;
                var13 = (var10 + 15.99F) / 256.0F;
                var14 = var11 / 256.0F;
                var15 = (var11 + 15.99F) / 256.0F;
            }
            else
            {
                var12 = var10 / 256.0F;
                var13 = (var10 + 15.99F) / 256.0F;
                var14 = (var11 + 16) / 256.0F;
                var15 = (var11 + 16 + 15.99F) / 256.0F;
            }

            if (var23 / 2 % 2 == 0)
            {
                (var13, var12) = (var12, var13);
            }

            var17.addVertexWithUV((double)(var18 - var19), (double)(0.0F - var21), (double)var22, (double)var13, (double)var15);
            var17.addVertexWithUV((double)(-var18 - var19), (double)(0.0F - var21), (double)var22, (double)var12, (double)var15);
            var17.addVertexWithUV((double)(-var18 - var19), (double)(1.4F - var21), (double)var22, (double)var12, (double)var14);
            var17.addVertexWithUV((double)(var18 - var19), (double)(1.4F - var21), (double)var22, (double)var13, (double)var14);
            var20 -= 0.45F;
            var21 -= 0.45F;
            var18 *= 0.9F;
            var22 += 0.03F;
            ++var23;
        }

        var17.draw();
        GLManager.GL.PopMatrix();
        GLManager.GL.Enable(GLEnum.Lighting);
    }

    private void renderShadow(Entity target, double x, double y, double z, float shadowiness, float tickDelta)
    {
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        TextureManager var10 = dispatcher.textureManager;
        var10.BindTexture(var10.GetTextureId("%clamp%/misc/shadow.png"));
        World var11 = getWorld();
        GLManager.GL.DepthMask(false);
        float radius = shadowRadius;
        double targetX = target.lastTickX + (target.x - target.lastTickX) * (double)tickDelta;
        double targetY = target.lastTickY + (target.y - target.lastTickY) * (double)tickDelta + (double)target.getShadowRadius();
        double targetZ = target.lastTickZ + (target.z - target.lastTickZ) * (double)tickDelta;
        int minX = MathHelper.Floor(targetX - (double)radius);
        int maxX = MathHelper.Floor(targetX + (double)radius);
        int minY = MathHelper.Floor(targetY - (double)radius);
        int maxY = MathHelper.Floor(targetY);
        int minZ = MathHelper.Floor(targetZ - (double)radius);
        int maxZ = MathHelper.Floor(targetZ + (double)radius);
        double dx = x - targetX;
        double dy = y - targetY;
        double dz = z - targetZ;
        Tessellator tess = Tessellator.instance;
        tess.startDrawingQuads();

        for (int blockX = minX; blockX <= maxX; ++blockX)
        {
            for (int blockY = minY; blockY <= maxY; ++blockY)
            {
                for (int blockZ = minZ; blockZ <= maxZ; ++blockZ)
                {
                    int var35 = var11.getBlockId(blockX, blockY - 1, blockZ);
                    if (var35 > 0 && var11.getLightLevel(blockX, blockY, blockZ) > 3)
                    {
                        renderShadowOnBlock(Block.Blocks[var35], x, y + (double)target.getShadowRadius(), z, blockX, blockY, blockZ, shadowiness, radius, dx, dy + (double)target.getShadowRadius(), dz);
                    }
                }
            }
        }

        tess.draw();
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        GLManager.GL.Disable(GLEnum.Blend);
        GLManager.GL.DepthMask(true);
    }

    private World getWorld()
    {
        return dispatcher.world;
    }

    private void renderShadowOnBlock(
        Block block,
        double x,
        double y,
        double z,
        int blockX,
        int blockY,
        int blockZ,
        float shadowiness,
        float radius,
        double dx,
        double dy,
        double dz)
    {
        Tessellator var19 = Tessellator.instance;
        if (block.isFullCube())
        {
            double shadowDarkness = ((double)shadowiness - (y - (blockY + dy)) / 2.0D) * 0.5D * (double)getWorld().getLuminance(blockX, blockY, blockZ);
            if (shadowDarkness >= 0.0D)
            {
                if (shadowDarkness > 1.0D)
                {
                    shadowDarkness = 1.0D;
                }

                var19.setColorRGBA_F(1.0F, 1.0F, 1.0F, (float)shadowDarkness);
                double minX = blockX + block.BoundingBox.minX + dx;
                double maxX = blockX + block.BoundingBox.maxX + dx;
                double minY = blockY + block.BoundingBox.minY + dy + 1.0D / 64.0D;
                double minZ = blockZ + block.BoundingBox.minZ + dz;
                double maxZ = blockZ + block.BoundingBox.maxZ + dz;
                float var32 = (float)((x - minX) / 2.0D / (double)radius + 0.5D);
                float var33 = (float)((x - maxX) / 2.0D / (double)radius + 0.5D);
                float var34 = (float)((z - minZ) / 2.0D / (double)radius + 0.5D);
                float var35 = (float)((z - maxZ) / 2.0D / (double)radius + 0.5D);
                var19.addVertexWithUV(minX, minY, minZ, (double)var32, (double)var34);
                var19.addVertexWithUV(minX, minY, maxZ, (double)var32, (double)var35);
                var19.addVertexWithUV(maxX, minY, maxZ, (double)var33, (double)var35);
                var19.addVertexWithUV(maxX, minY, minZ, (double)var33, (double)var34);
            }
        }
    }

    public static void renderShape(Box aabb, double x, double y, double z)
    {
        GLManager.GL.Disable(GLEnum.Texture2D);
        Tessellator var7 = Tessellator.instance;
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        var7.startDrawingQuads();
        var7.setTranslationD(x, y, z);
        var7.setNormal(0.0F, 0.0F, -1.0F);
        var7.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        var7.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        var7.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        var7.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        var7.setNormal(0.0F, 0.0F, 1.0F);
        var7.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        var7.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        var7.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        var7.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        var7.setNormal(0.0F, -1.0F, 0.0F);
        var7.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        var7.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        var7.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        var7.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        var7.setNormal(0.0F, 1.0F, 0.0F);
        var7.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        var7.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        var7.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        var7.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        var7.setNormal(-1.0F, 0.0F, 0.0F);
        var7.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        var7.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        var7.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        var7.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        var7.setNormal(1.0F, 0.0F, 0.0F);
        var7.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        var7.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        var7.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        var7.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        var7.setTranslationD(0.0D, 0.0D, 0.0D);
        var7.draw();
        GLManager.GL.Enable(GLEnum.Texture2D);
    }

    public static void renderShapeFlat(Box aabb)
    {
        Tessellator var1 = Tessellator.instance;
        var1.startDrawingQuads();
        var1.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        var1.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        var1.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        var1.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        var1.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        var1.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        var1.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        var1.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        var1.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        var1.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        var1.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        var1.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        var1.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        var1.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        var1.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        var1.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        var1.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        var1.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        var1.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        var1.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        var1.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        var1.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        var1.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        var1.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        var1.draw();
    }

    public void setDispatcher(EntityRenderDispatcher var1)
    {
        dispatcher = var1;
    }

    public void postRender(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        if (shadowRadius > 0.0F)
        {
            double distance = dispatcher.squareDistanceTo(target.x, target.y, target.z);
            float shadowiness = (float)((1.0D - distance / 256.0D) * shadowStrength);
            if (shadowiness > 0.0F)
            {
                renderShadow(target, x, y, z, shadowiness, tickDelta);
            }
        }

        if (target.isOnFire())
        {
            renderOnFire(target, x, y, z, tickDelta);
        }

    }

    public TextRenderer getTextRenderer()
    {
        return dispatcher.getTextRenderer();
    }
}
