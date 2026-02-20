using BetaSharp.Blocks;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities.Models;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using java.lang;

namespace BetaSharp.Client.Rendering.Entities;

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

    public void cacheActiveRenderInfo(World world, TextureManager textureManager, TextRenderer textRenderer, EntityLiving camera, GameOptions options, float tickDelta)
    {
        this.world = world;
        this.textureManager = textureManager;
        this.options = options;
        cameraEntity = camera;
        fontRenderer = textRenderer;
        if (camera.isSleeping())
        {
            int var7 = world.getBlockId(MathHelper.Floor(camera.x), MathHelper.Floor(camera.y), MathHelper.Floor(camera.z));
            if (var7 == Block.Bed.id)
            {
                int var8 = world.getBlockMeta(MathHelper.Floor(camera.x), MathHelper.Floor(camera.y), MathHelper.Floor(camera.z));
                int var9 = var8 & 3;
                playerViewY = var9 * 90 + 180;
                playerViewX = 0.0F;
            }
        }
        else
        {
            playerViewY = camera.prevYaw + (camera.yaw - camera.prevYaw) * tickDelta;
            playerViewX = camera.prevPitch + (camera.pitch - camera.prevPitch) * tickDelta;
        }

        x = camera.lastTickX + (camera.x - camera.lastTickX) * (double)tickDelta;
        y = camera.lastTickY + (camera.y - camera.lastTickY) * (double)tickDelta;
        z = camera.lastTickZ + (camera.z - camera.lastTickZ) * (double)tickDelta;
    }

    public void renderEntity(Entity target, float tickDelta)
    {
        double x = target.lastTickX + (target.x - target.lastTickX) * (double)tickDelta;
        double y = target.lastTickY + (target.y - target.lastTickY) * (double)tickDelta;
        double z = target.lastTickZ + (target.z - target.lastTickZ) * (double)tickDelta;
        float yaw = target.prevYaw + (target.yaw - target.prevYaw) * tickDelta;
        float brightness = target.getBrightnessAtEyes(tickDelta);
        GLManager.GL.Color3(brightness, brightness, brightness);
        renderEntityWithPosYaw(target, x - offsetX, y - offsetY, z - offsetZ, yaw, tickDelta);
    }

    public void renderEntityWithPosYaw(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        EntityRenderer var10 = getEntityRenderObject(target);
        if (var10 != null)
        {
            var10.render(target, x, y, z, yaw, tickDelta);
            var10.postRender(target, x, y, z, yaw, tickDelta);
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
