using BetaSharp.Blocks;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entitys.Models;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using java.lang;

namespace BetaSharp.Client.Rendering.Entitys;

public class EntityRenderDispatcher
{
    private readonly Dictionary<Class, EntityRenderer> entityRenderMap = [];
    public static EntityRenderDispatcher instance = new();
    private TextRenderer fontRenderer;
    public static double offsetX;
    public static double offsetY;
    public static double offsetZ;
    public TextureManager textureManager;
    public HeldItemRenderer heldItemRenderer;
    public World world;
    public EntityLiving cameraEntity;
    public float playerViewY;
    public float playerViewX;
    public GameOptions options;
    public double x;
    public double y;
    public double z;

    private EntityRenderDispatcher()
    {
        registerRenderer(EntitySpider.Class, new SpiderEntityRenderer());
        registerRenderer(EntityPig.Class, new PigEntityRenderer(new ModelPig(), new ModelPig(0.5F), 0.7F));
        registerRenderer(EntitySheep.Class, new SheepEntityRenderer(new ModelSheep2(), new ModelSheep1(), 0.7F));
        registerRenderer(EntityCow.Class, new CowEntityRenderer(new ModelCow(), 0.7F));
        registerRenderer(EntityWolf.Class, new WolfEntityRenderer(new ModelWolf(), 0.5F));
        registerRenderer(EntityChicken.Class, new ChickenEntityRenderer(new ModelChicken(), 0.3F));
        registerRenderer(EntityCreeper.Class, new CreeperEntityRenderer());
        registerRenderer(EntitySkeleton.Class, new UndeadEntityRenderer(new ModelSkeleton(), 0.5F));
        registerRenderer(EntityZombie.Class, new UndeadEntityRenderer(new ModelZombie(), 0.5F));
        registerRenderer(EntitySlime.Class, new SlimeEntityRenderer(new ModelSlime(16), new ModelSlime(0), 0.25F));
        registerRenderer(EntityPlayer.Class, new PlayerEntityRenderer());
        registerRenderer(EntityGiantZombie.Class, new GiantEntityRenderer(new ModelZombie(), 0.5F, 6.0F));
        registerRenderer(EntityGhast.Class, new GhastEntityRenderer());
        registerRenderer(EntitySquid.Class, new SquidEntityRenderer(new ModelSquid(), 0.7F));
        registerRenderer(EntityLiving.Class, new LivingEntityRenderer(new ModelBiped(), 0.5F));
        registerRenderer(Entity.Class, new BoxEntityRenderer());
        registerRenderer(EntityPainting.Class, new PaintingEntityRenderer());
        registerRenderer(EntityArrow.Class, new ArrowEntityRenderer());
        registerRenderer(EntitySnowball.Class, new ProjectileEntityRenderer(Item.Snowball.getTextureId(0)));
        registerRenderer(EntityEgg.Class, new ProjectileEntityRenderer(Item.Egg.getTextureId(0)));
        registerRenderer(EntityFireball.Class, new FireballEntityRenderer());
        registerRenderer(EntityItem.Class, new ItemRenderer());
        registerRenderer(EntityTNTPrimed.Class, new TntEntityRenderer());
        registerRenderer(EntityFallingSand.Class, new FallingBlockEntityRenderer());
        registerRenderer(EntityMinecart.Class, new MinecartEntityRenderer());
        registerRenderer(EntityBoat.Class, new BoatEntityRenderer());
        registerRenderer(EntityFish.Class, new FishingBobberEntityRenderer());
        registerRenderer(EntityLightningBolt.Class, new LightningEntityRenderer());

        foreach (var render in entityRenderMap.Values)
        {
            render.setDispatcher(this);
        }
    }

    private void registerRenderer(Class clazz, EntityRenderer render)
    {
        entityRenderMap[clazz] = render;
    }

    public EntityRenderer getEntityClassRenderObject(Class var1)
    {
        entityRenderMap.TryGetValue(var1, out EntityRenderer? var2);
        if (var2 == null && var1 != Entity.Class)
        {
            var2 = getEntityClassRenderObject(var1.getSuperclass());
            registerRenderer(var1, var2);
        }

        return var2;
    }

    public EntityRenderer getEntityRenderObject(Entity var1)
    {
        return getEntityClassRenderObject(var1.getClass());
    }

    public void cacheActiveRenderInfo(World var1, TextureManager var2, TextRenderer var3, EntityLiving var4, GameOptions var5, float var6)
    {
        world = var1;
        textureManager = var2;
        options = var5;
        cameraEntity = var4;
        fontRenderer = var3;
        if (var4.isSleeping())
        {
            int var7 = var1.getBlockId(MathHelper.floor_double(var4.x), MathHelper.floor_double(var4.y), MathHelper.floor_double(var4.z));
            if (var7 == Block.Bed.id)
            {
                int var8 = var1.getBlockMeta(MathHelper.floor_double(var4.x), MathHelper.floor_double(var4.y), MathHelper.floor_double(var4.z));
                int var9 = var8 & 3;
                playerViewY = var9 * 90 + 180;
                playerViewX = 0.0F;
            }
        }
        else
        {
            playerViewY = var4.prevYaw + (var4.yaw - var4.prevYaw) * var6;
            playerViewX = var4.prevPitch + (var4.pitch - var4.prevPitch) * var6;
        }

        x = var4.lastTickX + (var4.x - var4.lastTickX) * (double)var6;
        y = var4.lastTickY + (var4.y - var4.lastTickY) * (double)var6;
        z = var4.lastTickZ + (var4.z - var4.lastTickZ) * (double)var6;
    }

    public void renderEntity(Entity var1, float var2)
    {
        double var3 = var1.lastTickX + (var1.x - var1.lastTickX) * (double)var2;
        double var5 = var1.lastTickY + (var1.y - var1.lastTickY) * (double)var2;
        double var7 = var1.lastTickZ + (var1.z - var1.lastTickZ) * (double)var2;
        float var9 = var1.prevYaw + (var1.yaw - var1.prevYaw) * var2;
        float var10 = var1.getBrightnessAtEyes(var2);
        GLManager.GL.Color3(var10, var10, var10);
        renderEntityWithPosYaw(var1, var3 - offsetX, var5 - offsetY, var7 - offsetZ, var9, var2);
    }

    public void renderEntityWithPosYaw(Entity var1, double var2, double var4, double var6, float var8, float var9)
    {
        EntityRenderer var10 = getEntityRenderObject(var1);
        if (var10 != null)
        {
            var10.render(var1, var2, var4, var6, var8, var9);
            var10.postRender(var1, var2, var4, var6, var8, var9);
        }

    }

    public void func_852_a(World var1)
    {
        world = var1;
    }

    public double squareDistanceTo(double var1, double var3, double var5)
    {
        double var7 = var1 - x;
        double var9 = var3 - y;
        double var11 = var5 - z;
        return var7 * var7 + var9 * var9 + var11 * var11;
    }

    public TextRenderer getTextRenderer()
    {
        return fontRenderer;
    }
}