using BetaSharp.Blocks;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Biomes.Source;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Gen.Chunks;
using Silk.NET.Maths;

namespace BetaSharp.Worlds.Dimensions;

public abstract class Dimension : java.lang.Object
{
    public World world;
    public BiomeSource biomeSource;
    public bool isNether = false;
    public bool evaporatesWater = false;
    public bool hasCeiling = false;
    public float[] lightLevelToLuminance = new float[16];
    public int id = 0;
    private readonly float[] backgroundColor = new float[4];

    public void setWorld(World world)
    {
        this.world = world;
        initBiomeSource();
        initBrightnessTable();
    }

    protected virtual void initBrightnessTable()
    {
        float var1 = 0.05F;

        for (int var2 = 0; var2 <= 15; ++var2)
        {
            float var3 = 1.0F - var2 / 15.0F;
            lightLevelToLuminance[var2] = (1.0F - var3) / (var3 * 3.0F + 1.0F) * (1.0F - var1) + var1;
        }

    }

    public virtual void initBiomeSource()
    {
        biomeSource = new BiomeSource(world);
    }

    public virtual ChunkSource createChunkGenerator()
    {
        return new OverworldChunkGenerator(world, world.getSeed());
    }

    public virtual bool isValidSpawnPoint(int var1, int var2)
    {
        int var3 = world.getSpawnBlockId(var1, var2);
        return var3 == Block.Sand.id;
    }

    public virtual float getTimeOfDay(long time, float tickDelta)
    {
        int var4 = (int)(time % 24000L);
        float var5 = (var4 + tickDelta) / 24000.0F - 0.25F;
        if (var5 < 0.0F)
        {
            ++var5;
        }

        if (var5 > 1.0F)
        {
            --var5;
        }

        float var6 = var5;
        var5 = 1.0F - (float)((java.lang.Math.cos((double)var5 * java.lang.Math.PI) + 1.0D) / 2.0D);
        var5 = var6 + (var5 - var6) / 3.0F;
        return var5;
    }

    public virtual float[] getBackgroundColor(float var1, float var2)
    {
        float var3 = 0.4F;
        float var4 = MathHelper.Cos(var1 * (float)java.lang.Math.PI * 2.0F) - 0.0F;
        float var5 = -0.0F;
        if (var4 >= var5 - var3 && var4 <= var5 + var3)
        {
            float var6 = (var4 - var5) / var3 * 0.5F + 0.5F;
            float var7 = 1.0F - (1.0F - MathHelper.Sin(var6 * (float)java.lang.Math.PI)) * 0.99F;
            var7 *= var7;
            backgroundColor[0] = var6 * 0.3F + 0.7F;
            backgroundColor[1] = var6 * var6 * 0.7F + 0.2F;
            backgroundColor[2] = var6 * var6 * 0.0F + 0.2F;
            backgroundColor[3] = var7;
            return backgroundColor;
        }
        else
        {
            return null;
        }
    }

    public virtual Vector3D<double> getFogColor(float var1, float var2)
    {
        float var3 = MathHelper.Cos(var1 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F;
        if (var3 < 0.0F)
        {
            var3 = 0.0F;
        }

        if (var3 > 1.0F)
        {
            var3 = 1.0F;
        }

        float var4 = 192.0F / 255.0F;
        float var5 = 216.0F / 255.0F;
        float var6 = 1.0F;
        var4 *= var3 * 0.94F + 0.06F;
        var5 *= var3 * 0.94F + 0.06F;
        var6 *= var3 * 0.91F + 0.09F;
        return new((double)var4, (double)var5, (double)var6);
    }

    public virtual bool hasWorldSpawn()
    {
        return true;
    }

    public static Dimension fromId(int id)
    {
        return (Dimension)(id == -1 ? new NetherDimension() : id == 0 ? new OverworldDimension() : null);
    }

    public virtual float getCloudHeight()
    {
        return 108.0F;
    }

    public virtual bool hasGround()
    {
        return true;
    }
}