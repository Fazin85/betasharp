using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.Worlds;
using java.lang;
using java.util;

namespace BetaSharp.Client.Rendering.Blocks.Entities;

public class BlockEntityRenderer
{
    private Map specialRendererMap = new HashMap();
    public static BlockEntityRenderer instance = new BlockEntityRenderer();
    private TextRenderer fontRenderer;
    public static double staticPlayerX;
    public static double staticPlayerY;
    public static double staticPlayerZ;
    public TextureManager renderEngine;
    public World worldObj;
    public EntityLiving entityLivingPlayer;
    public float playerYaw;
    public float playerPitch;
    public double playerX;
    public double playerY;
    public double playerZ;

    private BlockEntityRenderer()
    {
        specialRendererMap.put(BlockEntitySign.Class, new BlockEntitySignRenderer());
        specialRendererMap.put(BlockEntityMobSpawner.Class, new BlockEntityMobSpawnerRenderer());
        specialRendererMap.put(BlockEntityPiston.Class, new BlockEntityRendererPiston());
        Iterator var1 = specialRendererMap.values().iterator();

        while (var1.hasNext())
        {
            BlockEntitySpecialRenderer var2 = (BlockEntitySpecialRenderer)var1.next();
            var2.setTileEntityRenderer(this);
        }

    }

    public BlockEntitySpecialRenderer getSpecialRendererForClass(Class var1)
    {
        BlockEntitySpecialRenderer var2 = (BlockEntitySpecialRenderer)specialRendererMap.get(var1);
        if (var2 == null && var1 != BlockEntity.Class)
        {
            var2 = getSpecialRendererForClass(var1.getSuperclass());
            specialRendererMap.put(var1, var2);
        }

        return var2;
    }

    public bool hasSpecialRenderer(BlockEntity var1)
    {
        return getSpecialRendererForEntity(var1) != null;
    }

    public BlockEntitySpecialRenderer getSpecialRendererForEntity(BlockEntity var1)
    {
        return var1 == null ? null : getSpecialRendererForClass(var1.getClass());
    }

    public void cacheActiveRenderInfo(World var1, TextureManager var2, TextRenderer var3, EntityLiving var4, float var5)
    {
        if (worldObj != var1)
        {
            func_31072_a(var1);
        }

        renderEngine = var2;
        entityLivingPlayer = var4;
        fontRenderer = var3;
        playerYaw = var4.prevYaw + (var4.yaw - var4.prevYaw) * var5;
        playerPitch = var4.prevPitch + (var4.pitch - var4.prevPitch) * var5;
        playerX = var4.lastTickX + (var4.x - var4.lastTickX) * (double)var5;
        playerY = var4.lastTickY + (var4.y - var4.lastTickY) * (double)var5;
        playerZ = var4.lastTickZ + (var4.z - var4.lastTickZ) * (double)var5;
    }

    public void renderTileEntity(BlockEntity var1, float var2)
    {
        if (var1.distanceFrom(playerX, playerY, playerZ) < 4096.0D)
        {
            float var3 = worldObj.getLuminance(var1.x, var1.y, var1.z);
            GLManager.GL.Color3(var3, var3, var3);
            renderTileEntityAt(var1, var1.x - staticPlayerX, var1.y - staticPlayerY, var1.z - staticPlayerZ, var2);
        }

    }

    public void renderTileEntityAt(BlockEntity var1, double var2, double var4, double var6, float var8)
    {
        BlockEntitySpecialRenderer var9 = getSpecialRendererForEntity(var1);
        if (var9 != null)
        {
            var9.renderTileEntityAt(var1, var2, var4, var6, var8);
        }

    }

    public void func_31072_a(World var1)
    {
        worldObj = var1;
        Iterator var2 = specialRendererMap.values().iterator();

        while (var2.hasNext())
        {
            BlockEntitySpecialRenderer var3 = (BlockEntitySpecialRenderer)var2.next();
            if (var3 != null)
            {
                var3.func_31069_a(var1);
            }
        }

    }

    public TextRenderer getFontRenderer()
    {
        return fontRenderer;
    }
}