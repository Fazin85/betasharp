using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entities;

public abstract class EntityRenderer
{
    protected EntityRenderDispatcher Dispatcher { get; set; } = null!;
    protected float ShadowRadius = 0.0F;
    protected float ShadowStrength = 1.0F;

    public abstract void render(Entity target, double x, double y, double z, float yaw, float tickDelta);

    protected void loadTexture(string path)
    {
        TextureManager? var2 = Dispatcher.textureManager;
        var2?.BindTexture(var2.GetTextureId(path));
    }

    protected bool LoadDownloadableImageTexture(string url, string fallbackPath)
    {
        //RenderEngine var3 = renderManager.renderEngine;
        if (string.IsNullOrEmpty(fallbackPath)) return false;

        loadTexture(fallbackPath);
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

    private void RenderOnFire(Entity ent, double x, double y, double z, float tickDelta)
    {
        GLManager.GL.Disable(GLEnum.Lighting);

        int textureId = Block.Fire.textureId;
        int texX = (textureId & 15) << 4;
        int texY = textureId & 240;

        float minU;
        float maxU;
        float minV;
        float maxV;

        GLManager.GL.PushMatrix();
        GLManager.GL.Translate((float)x, (float)y, (float)z);

        float scale = ent.width * 1.4F;
        GLManager.GL.Scale(scale, scale, scale);

        loadTexture("/terrain.png");
        Tessellator tess = Tessellator.instance;

        float widthOffset = 0.5F;
        float depthOffset = 0.0F;
        float heightRatio = ent.height / scale;
        float yOffset = (float)(ent.y - ent.boundingBox.minY);

        GLManager.GL.Rotate(-Dispatcher.playerViewY, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Translate(0.0F, 0.0F, -0.3F + (int)heightRatio * 0.02F);
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);

        float zOffset = 0.0F;
        int pass = 0;

        tess.startDrawingQuads();

        while (heightRatio > 0.0F)
        {
            if (pass % 2 == 0)
            {
                minU = texX / 256.0F;
                maxU = (texX + 15.99F) / 256.0F;
                minV = texY / 256.0F;
                maxV = (texY + 15.99F) / 256.0F;
            }
            else
            {
                minU = texX / 256.0F;
                maxU = (texX + 15.99F) / 256.0F;
                minV = (texY + 16) / 256.0F;
                maxV = (texY + 16 + 15.99F) / 256.0F;
            }

            if (pass / 2 % 2 == 0)
            {
                (maxU, minU) = (minU, maxU);
            }

            tess.addVertexWithUV((double)(widthOffset - depthOffset), (double)(0.0F - yOffset), (double)zOffset, (double)maxU, (double)maxV);
            tess.addVertexWithUV((double)(-widthOffset - depthOffset), (double)(0.0F - yOffset), (double)zOffset, (double)minU, (double)maxV);
            tess.addVertexWithUV((double)(-widthOffset - depthOffset), (double)(1.4F - yOffset), (double)zOffset, (double)minU, (double)minV);
            tess.addVertexWithUV((double)(widthOffset - depthOffset), (double)(1.4F - yOffset), (double)zOffset, (double)maxU, (double)minV);

            heightRatio -= 0.45F;
            yOffset -= 0.45F;
            widthOffset *= 0.9F;
            zOffset += 0.03F;
            ++pass;
        }

        tess.draw();
        GLManager.GL.PopMatrix();
        GLManager.GL.Enable(GLEnum.Lighting);
    }

    private void RenderShadow(Entity target, double x, double y, double z, float shadowiness, float tickDelta)
    {
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

        TextureManager textureManager = Dispatcher.textureManager;
        textureManager.BindTexture(textureManager.GetTextureId("%clamp%/misc/shadow.png"));
       // World world = getWorld();
        GLManager.GL.DepthMask(false);
        float radius = ShadowRadius;

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
                    int blockId = getWorld().getBlockId(blockX, blockY - 1, blockZ);
                    if (blockId > 0 && getWorld().getLightLevel(blockX, blockY, blockZ) > 3)
                    {
                        renderShadowOnBlock(Block.Blocks[blockId], x, y + (double)target.getShadowRadius(), z, blockX, blockY, blockZ, shadowiness, radius, dx, dy + (double)target.getShadowRadius(), dz);
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
        return Dispatcher.world;
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
        if (!block.isFullCube()) return;

        double shadowDarkness = ((double)shadowiness - (y - (blockY + dy)) / 2.0D) * 0.5D * (double)getWorld().getLuminance(blockX, blockY, blockZ);

        if (shadowDarkness < 0.0D) return;

        if (shadowDarkness > 1.0D)
        {
            shadowDarkness = 1.0D;
        }

        Tessellator tess = Tessellator.instance;
        tess.setColorRGBA_F(1.0F, 1.0F, 1.0F, (float)shadowDarkness);

        double minX = blockX + block.BoundingBox.minX + dx;
        double maxX = blockX + block.BoundingBox.maxX + dx;
        double minY = blockY + block.BoundingBox.minY + dy + 1.0D / 64.0D;
        double minZ = blockZ + block.BoundingBox.minZ + dz;
        double maxZ = blockZ + block.BoundingBox.maxZ + dz;

        float minU = (float)((x - minX) / 2.0D / (double)radius + 0.5D);
        float maxU = (float)((x - maxX) / 2.0D / (double)radius + 0.5D);
        float minV = (float)((z - minZ) / 2.0D / (double)radius + 0.5D);
        float maxV = (float)((z - maxZ) / 2.0D / (double)radius + 0.5D);

        tess.addVertexWithUV(minX, minY, minZ, (double)minU, (double)minV);
        tess.addVertexWithUV(minX, minY, maxZ, (double)minU, (double)maxV);
        tess.addVertexWithUV(maxX, minY, maxZ, (double)maxU, (double)maxV);
        tess.addVertexWithUV(maxX, minY, minZ, (double)maxU, (double)minV);
    }

    public static void renderShape(Box aabb, double x, double y, double z)
    {
        GLManager.GL.Disable(GLEnum.Texture2D);
        Tessellator tess = Tessellator.instance;
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);

        tess.startDrawingQuads();
        tess.setTranslationD(x, y, z);

        tess.setNormal(0.0F, 0.0F, -1.0F);

        tess.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.minX, aabb.minY, aabb.minZ);

        tess.setNormal(0.0F, 0.0F, 1.0F);
        tess.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);

        tess.setNormal(0.0F, -1.0F, 0.0F);
        tess.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.minY, aabb.maxZ);

        tess.setNormal(0.0F, 1.0F, 0.0F);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.minZ);

        tess.setNormal(-1.0F, 0.0F, 0.0F);
        tess.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.minX, aabb.minY, aabb.minZ);

        tess.setNormal(1.0F, 0.0F, 0.0F);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        
        tess.setTranslationD(0.0D, 0.0D, 0.0D);
        tess.draw();
        GLManager.GL.Enable(GLEnum.Texture2D);
    }

    public static void renderShapeFlat(Box aabb)
    {
        Tessellator tess = Tessellator.instance;
        tess.startDrawingQuads();

        tess.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.minX, aabb.minY, aabb.minZ);

        tess.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);

        tess.addVertex(aabb.minX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.minY, aabb.maxZ);

        tess.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.minZ);

        tess.addVertex(aabb.minX, aabb.minY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.minX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.minX, aabb.minY, aabb.minZ);

        tess.addVertex(aabb.maxX, aabb.minY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.minZ);
        tess.addVertex(aabb.maxX, aabb.maxY, aabb.maxZ);
        tess.addVertex(aabb.maxX, aabb.minY, aabb.maxZ);

        tess.draw();
    }

    public void PostRender(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        if (ShadowRadius > 0.0F)
        {
            double distance = Dispatcher.squareDistanceTo(target.x, target.y, target.z);
            float shadowiness = (float)((1.0D - distance / 256.0D) * ShadowStrength);
            if (shadowiness > 0.0F)
            {
                RenderShadow(target, x, y, z, shadowiness, tickDelta);
            }
        }

        if (target.isOnFire())
        {
            RenderOnFire(target, x, y, z, tickDelta);
        }

    }
}
