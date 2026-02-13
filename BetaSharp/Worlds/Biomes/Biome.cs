using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds.Gen.Features;
using java.awt;

namespace BetaSharp.Worlds.Biomes;

public class Biome
{
    public static readonly Biome RAINFOREST = new BiomeGenRainforest().setColor(588342).setBiomeName("Rainforest").func_4124_a(2094168);
    public static readonly Biome SWAMPLAND = new BiomeGenSwamp().setColor(522674).setBiomeName("Swampland").func_4124_a(9154376);
    public static readonly Biome SEASONAL_FOREST = new Biome().setColor(10215459).setBiomeName("Seasonal Forest");
    public static readonly Biome FOREST = new BiomeGenForest().setColor(353825).setBiomeName("Forest").func_4124_a(5159473);
    public static readonly Biome SAVANNA = new BiomeGenDesert().setColor(14278691).setBiomeName("Savanna");
    public static readonly Biome SHRUBLAND = new Biome().setColor(10595616).setBiomeName("Shrubland");
    public static readonly Biome TAIGA = new BiomeGenTaiga().setColor(3060051).setBiomeName("Taiga").setEnableSnow().func_4124_a(8107825);
    public static readonly Biome DESERT = new BiomeGenDesert().setColor(16421912).setBiomeName("Desert").setDisableRain();
    public static readonly Biome PLAINS = new BiomeGenDesert().setColor(16767248).setBiomeName("Plains");
    public static readonly Biome ICE_DESERT = new BiomeGenDesert().setColor(16772499).setBiomeName("Ice Desert").setEnableSnow().setDisableRain().func_4124_a(12899129);
    public static readonly Biome TUNDRA = new Biome().setColor(5762041).setBiomeName("Tundra").setEnableSnow().func_4124_a(12899129);
    public static readonly Biome HELL = new BiomeGenHell().setColor(16711680).setBiomeName("Hell").setDisableRain();
    public static readonly Biome SKY = new BiomeGenSky().setColor(8421631).setBiomeName("Sky").setDisableRain();
    public string name;
    public int grassColor;
    public byte topBlockId = (byte)Block.GRASS_BLOCK.id;
    public byte soilBlockId = (byte)Block.DIRT.id;
    public int foliageColor = 5169201;
    protected List<SpawnListEntry> spawnableMonsterList = [];
    protected List<SpawnListEntry> spawnableCreatureList = [];
    protected List<SpawnListEntry> spawnableWaterCreatureList = [];
    private bool hasSnow;
    private bool hasRain = true;
    private static Biome[] BIOMES = new Biome[4096];

    protected Biome()
    {
        spawnableMonsterList.Add(new SpawnListEntry(EntitySpider.Class, 10));
        spawnableMonsterList.Add(new SpawnListEntry(EntityZombie.Class, 10));
        spawnableMonsterList.Add(new SpawnListEntry(EntitySkeleton.Class, 10));
        spawnableMonsterList.Add(new SpawnListEntry(EntityCreeper.Class, 10));
        spawnableMonsterList.Add(new SpawnListEntry(EntitySlime.Class, 10));
        spawnableCreatureList.Add(new SpawnListEntry(EntitySheep.Class, 12));
        spawnableCreatureList.Add(new SpawnListEntry(EntityPig.Class, 10));
        spawnableCreatureList.Add(new SpawnListEntry(EntityChicken.Class, 10));
        spawnableCreatureList.Add(new SpawnListEntry(EntityCow.Class, 8));
        spawnableWaterCreatureList.Add(new SpawnListEntry(EntitySquid.Class, 10));
    }

    private Biome setDisableRain()
    {
        hasRain = false;
        return this;
    }

    public static void init()
    {
        for (int var0 = 0; var0 < 64; ++var0)
        {
            for (int var1 = 0; var1 < 64; ++var1)
            {
                BIOMES[var0 + var1 * 64] = locateBiome(var0 / 63.0F, var1 / 63.0F);
            }
        }

        DESERT.topBlockId = DESERT.soilBlockId = (byte)Block.SAND.id;
        ICE_DESERT.topBlockId = ICE_DESERT.soilBlockId = (byte)Block.SAND.id;
    }

    public virtual Feature getRandomWorldGenForTrees(java.util.Random var1)
    {
        return var1.nextInt(10) == 0 ? new LargeOakTreeFeature() : new OakTreeFeature();
    }

    protected Biome setEnableSnow()
    {
        hasSnow = true;
        return this;
    }

    protected Biome setBiomeName(string var1)
    {
        name = var1;
        return this;
    }

    protected Biome func_4124_a(int var1)
    {
        foliageColor = var1;
        return this;
    }

    protected Biome setColor(int var1)
    {
        grassColor = var1;
        return this;
    }

    public static Biome getBiome(double temp, double downfall)
    {
        int var4 = (int)(temp * 63.0D);
        int var5 = (int)(downfall * 63.0D);
        return BIOMES[var4 + var5 * 64];
    }

    public static Biome locateBiome(float temp, float downfall)
    {
        downfall *= temp;
        return temp < 0.1F ? TUNDRA : downfall < 0.2F ? temp < 0.5F ? TUNDRA : temp < 0.95F ? SAVANNA : DESERT : downfall > 0.5F && temp < 0.7F ? SWAMPLAND : temp < 0.5F ? TAIGA : temp < 0.97F ? downfall < 0.35F ? SHRUBLAND : FOREST : downfall < 0.45F ? PLAINS : downfall < 0.9F ? SEASONAL_FOREST : RAINFOREST;
    }

    public virtual int getSkyColorByTemp(float var1)
    {
        var1 /= 3.0F;
        if (var1 < -1.0F)
        {
            var1 = -1.0F;
        }

        if (var1 > 1.0F)
        {
            var1 = 1.0F;
        }

        return Color.getHSBColor(224.0F / 360.0F - var1 * 0.05F, 0.5F + var1 * 0.1F, 1.0F).getRGB();
    }

    public List<SpawnListEntry> getSpawnableList(EnumCreatureType var1)
    {
        return var1 == EnumCreatureType.monster ? spawnableMonsterList : var1 == EnumCreatureType.creature ? spawnableCreatureList : var1 == EnumCreatureType.waterCreature ? spawnableWaterCreatureList : null;
    }

    public bool getEnableSnow()
    {
        return hasSnow;
    }

    public bool canSpawnLightningBolt()
    {
        return hasSnow ? false : hasRain;
    }


    static Biome()
    {
        init();
    }
}