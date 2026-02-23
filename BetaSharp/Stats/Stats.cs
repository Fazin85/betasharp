using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Recipes;
using java.lang;
using java.util;

namespace BetaSharp.Stats;

public class Stats
{
    public static Dictionary<int, StatBase> IdToStat = [];
    public static List<StatBase> AllStats = [];
    public static List<StatBase> GeneralStats = [];
    public static List<StatBase> ItemStats = [];
    public static List<StatBase> BlocksMinedStats = [];

   public static StatBase StartGameStat = new StatBasic(1000, StatCollector.TranslateToLocal("stat.startGame")).SetLocalOnly().RegisterStat();
    public static StatBase CreateWorldStat = new StatBasic(1001, StatCollector.TranslateToLocal("stat.createWorld")).SetLocalOnly().RegisterStat();
    public static StatBase LoadWorldStat = new StatBasic(1002, StatCollector.TranslateToLocal("stat.loadWorld")).SetLocalOnly().RegisterStat();
    public static StatBase JoinMultiplayerStat = new StatBasic(1003, StatCollector.TranslateToLocal("stat.joinMultiplayer")).SetLocalOnly().RegisterStat();
    public static StatBase LeaveGameStat = new StatBasic(1004, StatCollector.TranslateToLocal("stat.leaveGame")).SetLocalOnly().RegisterStat();
    public static StatBase MinutesPlayedStat = new StatBasic(1100, StatCollector.TranslateToLocal("stat.playOneMinute"), StatFormatters.FormatTime).SetLocalOnly().RegisterStat();
    public static StatBase DistanceWalkedStat = new StatBasic(2000, StatCollector.TranslateToLocal("stat.walkOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceSwumStat = new StatBasic(2001, StatCollector.TranslateToLocal("stat.swimOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceFallenStat = new StatBasic(2002, StatCollector.TranslateToLocal("stat.fallOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceClimbedStat = new StatBasic(2003, StatCollector.TranslateToLocal("stat.climbOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceFlownStat = new StatBasic(2004, StatCollector.TranslateToLocal("stat.flyOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceDoveStat = new StatBasic(2005, StatCollector.TranslateToLocal("stat.diveOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceByMinecartStat = new StatBasic(2006, StatCollector.TranslateToLocal("stat.minecartOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceByBoatStat = new StatBasic(2007, StatCollector.TranslateToLocal("stat.boatOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase DistanceByPigStat = new StatBasic(2008, StatCollector.TranslateToLocal("stat.pigOneCm"), StatFormatters.FormatDistance).SetLocalOnly().RegisterStat();
    public static StatBase JumpStat = new StatBasic(2010, StatCollector.TranslateToLocal("stat.jump")).SetLocalOnly().RegisterStat();
    public static StatBase DropStat = new StatBasic(2011, StatCollector.TranslateToLocal("stat.drop")).SetLocalOnly().RegisterStat();
    public static StatBase DamageDealtStat = new StatBasic(2020, StatCollector.TranslateToLocal("stat.damageDealt")).RegisterStat();
    public static StatBase DamageTakenStat = new StatBasic(2021, StatCollector.TranslateToLocal("stat.damageTaken")).RegisterStat();
    public static StatBase DeathsStat = new StatBasic(2022, StatCollector.TranslateToLocal("stat.deaths")).RegisterStat();
    public static StatBase MobKillsStat = new StatBasic(2023, StatCollector.TranslateToLocal("stat.mobKills")).RegisterStat();
    public static StatBase PlayerKillsStat = new StatBasic(2024, StatCollector.TranslateToLocal("stat.playerKills")).RegisterStat();
    public static StatBase FishCaughtStat = new StatBasic(2025, StatCollector.TranslateToLocal("stat.fishCaught")).RegisterStat();

    public static StatBase[] MineBlockStatArray = initBlocksMined("stat.mineBlock", 16777216);
    public static StatBase[] Crafted;
    public static StatBase[] Used;
    public static StatBase[] Broken;
    
    private static bool _hasBasicItemStatsInitialized;
    private static bool _hasExtendedItemStatsInitialized;

    public static void initializeItemStats()
    {
        Used = initItemUsedStats(Used, "stat.useItem", 16908288, 0, Block.Blocks.Length);
        Broken = initializeBrokenItemStats(Broken, "stat.breakItem", 16973824, 0, Block.Blocks.Length);
        _hasBasicItemStatsInitialized = true;
        initializeCraftedItemStats();
    }

    public static void initializeExtendedItemStats()
    {
        Used = initItemUsedStats(Used, "stat.useItem", 16908288, Block.Blocks.Length, 32000);
        Broken = initializeBrokenItemStats(Broken, "stat.breakItem", 16973824, Block.Blocks.Length, 32000);
        _hasExtendedItemStatsInitialized = true;
        initializeCraftedItemStats();
    }

    public static void initializeCraftedItemStats()
    {
        if (_hasBasicItemStatsInitialized && _hasExtendedItemStatsInitialized)
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

            Crafted = new StatBase[32000];
            var1 = var0.iterator();

            while (var1 is Integer integer)
            {
                if (Item.ITEMS[integer.intValue()] != null)
                {
                    string var3 = StatCollector.TranslateToLocalFormatted("stat.craftItem", Item.ITEMS[integer.intValue()].getStatName());
                    Crafted[integer.intValue()] = (new StatCrafting(16842752 + integer.intValue(), var3, integer.intValue())).RegisterStat();
                }
            }

            replaceAllSimilarBlocks(Crafted);
        }
    }

    private static StatBase[] initBlocksMined(string var0, int var1)
    {
        StatBase[] var2 = new StatBase[256];

        for (int var3 = 0; var3 < 256; ++var3)
        {
            if (Block.Blocks[var3] != null && Block.Blocks[var3].getEnableStats())
            {
                string var4 = StatCollector.TranslateToLocalFormatted(var0, Block.Blocks[var3].translateBlockName());
                var2[var3] = (new StatCrafting(var1 + var3, var4, var3)).RegisterStat();
                BlocksMinedStats.Add((StatCrafting)var2[var3]);
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
                string var6 = StatCollector.TranslateToLocalFormatted(var1, Item.ITEMS[var5].getStatName());
                var0[var5] = (new StatCrafting(var2 + var5, var6, var5)).RegisterStat();
                if (var5 >= Block.Blocks.Length)
                {
                    ItemStats.Add((StatCrafting)var0[var5]);
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
                string var6 = StatCollector.TranslateToLocalFormatted(var1, Item.ITEMS[var5].getStatName());
                var0[var5] = (new StatCrafting(var2 + var5, var6, var5)).RegisterStat();
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
            AllStats.Remove(var0[var1]);
            BlocksMinedStats.Remove(var0[var1]);
            GeneralStats.Remove(var0[var1]);
            var0[var1] = var0[var2];
        }
    }

    public static StatBase getStatById(int var0)
    {
        return IdToStat[var0];
    }

    static Stats()
    {
        BetaSharp.Achievements.initialize();
    }
}