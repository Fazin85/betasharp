using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Recipes;
using java.lang;
using java.util;

namespace BetaSharp.Stats;

public class Stats : java.lang.Object
{
    public static Map ID_TO_STAT = new HashMap();
    public static List ALL_STATS = new ArrayList();
    public static List GENERAL_STATS = new ArrayList();
    public static List ITEM_STATS = new ArrayList();
    public static List BLOCKS_MINED_STATS = new ArrayList();
    public static StatBase startGameStat = (new StatBasic(1000, StatCollector.translateToLocal("stat.startGame"))).setLocalOnly().registerStat();
    public static StatBase createWorldStat = (new StatBasic(1001, StatCollector.translateToLocal("stat.createWorld"))).setLocalOnly().registerStat();
    public static StatBase loadWorldStat = (new StatBasic(1002, StatCollector.translateToLocal("stat.loadWorld"))).setLocalOnly().registerStat();
    public static StatBase joinMultiplayerStat = (new StatBasic(1003, StatCollector.translateToLocal("stat.joinMultiplayer"))).setLocalOnly().registerStat();
    public static StatBase leaveGameStat = (new StatBasic(1004, StatCollector.translateToLocal("stat.leaveGame"))).setLocalOnly().registerStat();
    public static StatBase minutesPlayedStat = (new StatBasic(1100, StatCollector.translateToLocal("stat.playOneMinute"), StatBase.TIME_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceWalkedStat = (new StatBasic(2000, StatCollector.translateToLocal("stat.walkOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceSwumStat = (new StatBasic(2001, StatCollector.translateToLocal("stat.swimOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceFallenStat = (new StatBasic(2002, StatCollector.translateToLocal("stat.fallOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceClimbedStat = (new StatBasic(2003, StatCollector.translateToLocal("stat.climbOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceFlownStat = (new StatBasic(2004, StatCollector.translateToLocal("stat.flyOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceDoveStat = (new StatBasic(2005, StatCollector.translateToLocal("stat.diveOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceByMinecartStat = (new StatBasic(2006, StatCollector.translateToLocal("stat.minecartOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceByBoatStat = (new StatBasic(2007, StatCollector.translateToLocal("stat.boatOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase distanceByPigStat = (new StatBasic(2008, StatCollector.translateToLocal("stat.pigOneCm"), StatBase.DISTANCE_PROVIDER)).setLocalOnly().registerStat();
    public static StatBase jumpStat = (new StatBasic(2010, StatCollector.translateToLocal("stat.jump"))).setLocalOnly().registerStat();
    public static StatBase dropStat = (new StatBasic(2011, StatCollector.translateToLocal("stat.drop"))).setLocalOnly().registerStat();
    public static StatBase damageDealtStat = (new StatBasic(2020, StatCollector.translateToLocal("stat.damageDealt"))).registerStat();
    public static StatBase damageTakenStat = (new StatBasic(2021, StatCollector.translateToLocal("stat.damageTaken"))).registerStat();
    public static StatBase deathsStat = (new StatBasic(2022, StatCollector.translateToLocal("stat.deaths"))).registerStat();
    public static StatBase mobKillsStat = (new StatBasic(2023, StatCollector.translateToLocal("stat.mobKills"))).registerStat();
    public static StatBase playerKillsStat = (new StatBasic(2024, StatCollector.translateToLocal("stat.playerKills"))).registerStat();
    public static StatBase fishCaughtStat = (new StatBasic(2025, StatCollector.translateToLocal("stat.fishCaught"))).registerStat();
    public static StatBase[] mineBlockStatArray = initBlocksMined("stat.mineBlock", 16777216);
    public static StatBase[] CRAFTED;
    public static StatBase[] USED;
    public static StatBase[] BROKEN;
    private static bool hasBasicItemStatsInitialized;
    private static bool hasExtendedItemStatsInitialized;

    public static void initializeItemStats()
    {
        USED = initItemUsedStats(USED, "stat.useItem", 16908288, 0, Block.Blocks.Length);
        BROKEN = initializeBrokenItemStats(BROKEN, "stat.breakItem", 16973824, 0, Block.Blocks.Length);
        hasBasicItemStatsInitialized = true;
        initializeCraftedItemStats();
    }

    public static void initializeExtendedItemStats()
    {
        USED = initItemUsedStats(USED, "stat.useItem", 16908288, Block.Blocks.Length, 32000);
        BROKEN = initializeBrokenItemStats(BROKEN, "stat.breakItem", 16973824, Block.Blocks.Length, 32000);
        hasExtendedItemStatsInitialized = true;
        initializeCraftedItemStats();
    }

    public static void initializeCraftedItemStats()
    {
        if (hasBasicItemStatsInitialized && hasExtendedItemStatsInitialized)
        {
            HashSet var0 = new HashSet();
            object var1 = CraftingManager.getInstance().Recipes.GetEnumerator();

            while (var1 is IRecipe recipe)
            {
                var0.add(Integer.valueOf(recipe.GetRecipeOutput().itemId));
            }

            var1 = SmeltingRecipeManager.getInstance().GetSmeltingList().Values.GetEnumerator();

            while (var1 is ItemStack itemStack)
            {
                var0.add(Integer.valueOf(itemStack.itemId));
            }

            CRAFTED = new StatBase[32000];
            var1 = var0.iterator();

            while (var1 is Integer integer)
            {
                if (Item.ITEMS[integer.intValue()] != null)
                {
                    string var3 = StatCollector.translateToLocalFormatted("stat.craftItem", Item.ITEMS[integer.intValue()].getStatName());
                    CRAFTED[integer.intValue()] = (new StatCrafting(16842752 + integer.intValue(), var3, integer.intValue())).registerStat();
                }
            }

            replaceAllSimilarBlocks(CRAFTED);
        }
    }

    private static StatBase[] initBlocksMined(string var0, int var1)
    {
        StatBase[] var2 = new StatBase[256];

        for (int var3 = 0; var3 < 256; ++var3)
        {
            if (Block.Blocks[var3] != null && Block.Blocks[var3].getEnableStats())
            {
                string var4 = StatCollector.translateToLocalFormatted(var0, Block.Blocks[var3].translateBlockName());
                var2[var3] = (new StatCrafting(var1 + var3, var4, var3)).registerStat();
                BLOCKS_MINED_STATS.add((StatCrafting)var2[var3]);
            }
        }

        replaceAllSimilarBlocks(var2);
        return var2;
    }

    private static StatBase[] initItemUsedStats(StatBase[] var0, string var1, int var2, int var3, int var4)
    {
        if (var0 == null)
        {
            var0 = new StatBase[32000];
        }

        for (int var5 = var3; var5 < var4; ++var5)
        {
            if (Item.ITEMS[var5] != null)
            {
                string var6 = StatCollector.translateToLocalFormatted(var1, Item.ITEMS[var5].getStatName());
                var0[var5] = (new StatCrafting(var2 + var5, var6, var5)).registerStat();
                if (var5 >= Block.Blocks.Length)
                {
                    ITEM_STATS.add((StatCrafting)var0[var5]);
                }
            }
        }

        replaceAllSimilarBlocks(var0);
        return var0;
    }

    private static StatBase[] initializeBrokenItemStats(StatBase[] var0, string var1, int var2, int var3, int var4)
    {
        if (var0 == null)
        {
            var0 = new StatBase[32000];
        }

        for (int var5 = var3; var5 < var4; ++var5)
        {
            if (Item.ITEMS[var5] != null && Item.ITEMS[var5].isDamagable())
            {
                string var6 = StatCollector.translateToLocalFormatted(var1, Item.ITEMS[var5].getStatName());
                var0[var5] = (new StatCrafting(var2 + var5, var6, var5)).registerStat();
            }
        }

        replaceAllSimilarBlocks(var0);
        return var0;
    }

    private static void replaceAllSimilarBlocks(StatBase[] var0)
    {
        replaceSimilarBlocks(var0, Block.Water.id, Block.FlowingWater.id);
        replaceSimilarBlocks(var0, Block.Lava.id, Block.Lava.id);
        replaceSimilarBlocks(var0, Block.JackLantern.id, Block.Pumpkin.id);
        replaceSimilarBlocks(var0, Block.LitFurnace.id, Block.Furnace.id);
        replaceSimilarBlocks(var0, Block.LitRedstoneOre.id, Block.RedstoneOre.id);
        replaceSimilarBlocks(var0, Block.PoweredRepeater.id, Block.Repeater.id);
        replaceSimilarBlocks(var0, Block.LitRedstoneTorch.id, Block.RedstoneTorch.id);
        replaceSimilarBlocks(var0, Block.RedMushroom.id, Block.BrownMushroom.id);
        replaceSimilarBlocks(var0, Block.DoubleSlab.id, Block.Slab.id);
        replaceSimilarBlocks(var0, Block.GrassBlock.id, Block.Dirt.id);
        replaceSimilarBlocks(var0, Block.Farmland.id, Block.Dirt.id);
    }

    private static void replaceSimilarBlocks(StatBase[] var0, int var1, int var2)
    {
        if (var0[var1] != null && var0[var2] == null)
        {
            var0[var2] = var0[var1];
        }
        else
        {
            ALL_STATS.remove(var0[var1]);
            BLOCKS_MINED_STATS.remove(var0[var1]);
            GENERAL_STATS.remove(var0[var1]);
            var0[var1] = var0[var2];
        }
    }

    public static StatBase getStatById(int var0)
    {
        return (StatBase)ID_TO_STAT.get(Integer.valueOf(var0));
    }

    static Stats()
    {
        BetaSharp.Achievements.initialize();
    }
}