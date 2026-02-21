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
    private readonly Dictionary<Type, EntityRenderer> entityRenderMap = [];
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
        registerRenderer(typeof(EntitySpider), new SpiderEntityRenderer());
        registerRenderer(typeof(EntityPig), new PigEntityRenderer(new ModelPig(), new ModelPig(0.5F), 0.7F));
        registerRenderer(typeof(EntitySheep), new SheepEntityRenderer(new ModelSheep2(), new ModelSheep1(), 0.7F));
        registerRenderer(typeof(EntityCow), new CowEntityRenderer(new ModelCow(), 0.7F));
        registerRenderer(typeof(EntityWolf), new WolfEntityRenderer(new ModelWolf(), 0.5F));
        registerRenderer(typeof(EntityChicken), new ChickenEntityRenderer(new ModelChicken(), 0.3F));
        registerRenderer(typeof(EntityCreeper), new CreeperEntityRenderer());
        registerRenderer(typeof(EntitySkeleton), new UndeadEntityRenderer(new ModelSkeleton(), 0.5F));
        registerRenderer(typeof(EntityZombie), new UndeadEntityRenderer(new ModelZombie(), 0.5F));
        registerRenderer(typeof(EntitySlime), new SlimeEntityRenderer(new ModelSlime(16), new ModelSlime(0), 0.25F));
        registerRenderer(typeof(EntityPlayer), new PlayerEntityRenderer());
        registerRenderer(typeof(EntityGiantZombie), new GiantEntityRenderer(new ModelZombie(), 0.5F, 6.0F));
        registerRenderer(typeof(EntityGhast), new GhastEntityRenderer());
        registerRenderer(typeof(EntitySquid), new SquidEntityRenderer(new ModelSquid(), 0.7F));
        registerRenderer(typeof(EntityLiving), new LivingEntityRenderer(new ModelBiped(), 0.5F));
        registerRenderer(typeof(Entity), new BoxEntityRenderer());
        registerRenderer(typeof(EntityPainting), new PaintingEntityRenderer());
        registerRenderer(typeof(EntityArrow), new ArrowEntityRenderer());
        registerRenderer(typeof(EntitySnowball), new ProjectileEntityRenderer(Item.Snowball.getTextureId(0)));
        registerRenderer(typeof(EntityEgg), new ProjectileEntityRenderer(Item.Egg.getTextureId(0)));
        registerRenderer(typeof(EntityFireball), new FireballEntityRenderer());
        registerRenderer(typeof(EntityItem), new ItemRenderer());
        registerRenderer(typeof(EntityTNTPrimed), new TntEntityRenderer());
        registerRenderer(typeof(EntityFallingSand), new FallingBlockEntityRenderer());
        registerRenderer(typeof(EntityMinecart), new MinecartEntityRenderer());
        registerRenderer(typeof(EntityBoat), new BoatEntityRenderer());
        registerRenderer(typeof(EntityFish), new FishingBobberEntityRenderer());
        registerRenderer(typeof(EntityLightningBolt), new LightningEntityRenderer());

        foreach (var render in entityRenderMap.Values)
        {
            render.setDispatcher(this);
        }
    }

    private void registerRenderer(Type type, EntityRenderer render)
    {
        entityRenderMap[type] = render;
    }

    public EntityRenderer getEntityClassRenderObject(Type type)
    {
        entityRenderMap.TryGetValue(type, out EntityRenderer? var2);
        if (var2 == null && type != typeof(Entity))
        {
            var2 = getEntityClassRenderObject(type.BaseType);
            registerRenderer(type, var2);
        }

        return var2;
    }

    public EntityRenderer getEntityRenderObject(Entity entity)
    {
        return getEntityClassRenderObject(entity.GetType());
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
