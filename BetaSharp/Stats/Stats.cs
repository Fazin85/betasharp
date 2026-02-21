using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Recipes;

namespace BetaSharp.Stats;

public class Stats
{
    public static readonly Dictionary<int, StatBase> IdToStat = new();
    public static readonly List<StatBase> AllStats = new();
    public static readonly List<StatBase> GeneralStats = new();
    public static readonly List<StatBase> ItemStats = new();
    public static readonly List<StatBase> BlockMinedStats = new();

    public static StatBase startGameStat = new StatBasic(1000, StatCollector.translateToLocal("stat.startGame")).SetLocalOnly().RegisterStat();
    public static StatBase createWorldStat = new StatBasic(1001, StatCollector.translateToLocal("stat.createWorld")).SetLocalOnly().RegisterStat();
    public static StatBase loadWorldStat = new StatBasic(1002, StatCollector.translateToLocal("stat.loadWorld")).SetLocalOnly().RegisterStat();
    public static StatBase joinMultiplayerStat = new StatBasic(1003, StatCollector.translateToLocal("stat.joinMultiplayer")).SetLocalOnly().RegisterStat();
    public static StatBase leaveGameStat = new StatBasic(1004, StatCollector.translateToLocal("stat.leaveGame")).SetLocalOnly().RegisterStat();
    public static StatBase minutesPlayedStat = new StatBasic(1100, StatCollector.translateToLocal("stat.playOneMinute"), StatBase.TimeFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceWalkedStat = new StatBasic(2000, StatCollector.translateToLocal("stat.walkOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceSwumStat = new StatBasic(2001, StatCollector.translateToLocal("stat.swimOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceFallenStat = new StatBasic(2002, StatCollector.translateToLocal("stat.fallOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceClimbedStat = new StatBasic(2003, StatCollector.translateToLocal("stat.climbOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceFlownStat = new StatBasic(2004, StatCollector.translateToLocal("stat.flyOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceDoveStat = new StatBasic(2005, StatCollector.translateToLocal("stat.diveOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceByMinecartStat = new StatBasic(2006, StatCollector.translateToLocal("stat.minecartOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceByBoatStat = new StatBasic(2007, StatCollector.translateToLocal("stat.boatOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase distanceByPigStat = new StatBasic(2008, StatCollector.translateToLocal("stat.pigOneCm"), StatBase.DistanceFormat).SetLocalOnly().RegisterStat();
    public static StatBase jumpStat = new StatBasic(2010, StatCollector.translateToLocal("stat.jump")).SetLocalOnly().RegisterStat();
    public static StatBase dropStat = new StatBasic(2011, StatCollector.translateToLocal("stat.drop")).SetLocalOnly().RegisterStat();
    public static StatBase damageDealtStat = new StatBasic(2020, StatCollector.translateToLocal("stat.damageDealt")).RegisterStat();
    public static StatBase damageTakenStat = new StatBasic(2021, StatCollector.translateToLocal("stat.damageTaken")).RegisterStat();
    public static StatBase deathsStat = new StatBasic(2022, StatCollector.translateToLocal("stat.deaths")).RegisterStat();
    public static StatBase mobKillsStat = new StatBasic(2023, StatCollector.translateToLocal("stat.mobKills")).RegisterStat();
    public static StatBase playerKillsStat = new StatBasic(2024, StatCollector.translateToLocal("stat.playerKills")).RegisterStat();
    public static StatBase fishCaughtStat = new StatBasic(2025, StatCollector.translateToLocal("stat.fishCaught")).RegisterStat();

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
            var validIds = new HashSet<int>();
            foreach (var recipe in CraftingManager.getInstance().Recipes)
            {
                validIds.Add(recipe.GetRecipeOutput().itemId);
            }

             foreach (var itemStack in SmeltingRecipeManager.getInstance().GetSmeltingList().Values)
            {
                validIds.Add(itemStack.itemId);
            }

            Crafted = new StatBase[32000];
            foreach (int id in validIds)
            {
                if (Item.ITEMS[id] != null)
                {
                    string name = TranslationStorage.getInstance().translateKeyFormat("stat.craftItem", Item.ITEMS[id].getStatName());
                    Crafted[id] = new StatCrafting(16842752 + id, name, id).RegisterStat();
                }
            }

            replaceAllSimilarBlocks(Crafted);
        }
    }

    private static StatBase[] initBlocksMined(string prefix, int baseId)
    {
        StatBase[] var2 = new StatBase[256];

        for (int i = 0; i < 256; ++i)
        {
            if (Block.Blocks[i] != null && Block.Blocks[i].getEnableStats())
            {
                string var4 = StatCollector.translateToLocalFormatted(prefix, Block.Blocks[i].translateBlockName());
                var2[i] = new StatCrafting(baseId + i, var4, i).RegisterStat();
                BlockMinedStats.Add((StatCrafting)var2[i]);
            }
        }

        replaceAllSimilarBlocks(var2);
        return var2;
    }

    private static StatBase[] initItemUsedStats(StatBase[] array, string prefix, int baseId, int start, int end)
    {
        if (array == null)
        {
            array = new StatBase[32000];
        }

        for (int i = start; i < end; ++i)
        {
            if (Item.ITEMS[i] != null)
            {
                string var6 = StatCollector.translateToLocalFormatted(prefix, Item.ITEMS[i].getStatName());
                array[i] = new StatCrafting(baseId + i, var6, i).RegisterStat();
                if (i >= Block.Blocks.Length)
                {
                    ItemStats.Add(array[i]);
                }
            }
        }

        replaceAllSimilarBlocks(array);
        return array;
    }

    private static StatBase[] initializeBrokenItemStats(StatBase[] array, string prefix, int baseId, int start, int end)
    {
        if (array == null)
        {
            array = new StatBase[32000];
        }

        for (int i = start; i < end; ++i)
        {
            if (Item.ITEMS[i] != null && Item.ITEMS[i].isDamagable())
            {
                string var6 = StatCollector.translateToLocalFormatted(prefix, Item.ITEMS[i].getStatName());
                array[i] = new StatCrafting(baseId + i, var6, i).RegisterStat();
            }
        }

        replaceAllSimilarBlocks(array);
        return array;
    }

    private static void replaceAllSimilarBlocks(StatBase[] stats)
    {
        ReplaceSimilar(stats, Block.Water.id, Block.FlowingWater.id);
        ReplaceSimilar(stats, Block.Lava.id, Block.Lava.id);
        ReplaceSimilar(stats, Block.JackLantern.id, Block.Pumpkin.id);
        ReplaceSimilar(stats, Block.LitFurnace.id, Block.Furnace.id);
        ReplaceSimilar(stats, Block.LitRedstoneOre.id, Block.RedstoneOre.id);
        ReplaceSimilar(stats, Block.PoweredRepeater.id, Block.Repeater.id);
        ReplaceSimilar(stats, Block.LitRedstoneTorch.id, Block.RedstoneTorch.id);
        ReplaceSimilar(stats, Block.RedMushroom.id, Block.BrownMushroom.id);
        ReplaceSimilar(stats, Block.DoubleSlab.id, Block.Slab.id);
        ReplaceSimilar(stats, Block.GrassBlock.id, Block.Dirt.id);
        ReplaceSimilar(stats, Block.Farmland.id, Block.Dirt.id);
    }

    private static void ReplaceSimilar(StatBase[] stats, int idFrom, int idTo)
    {
        if (stats[idFrom] != null && stats[idTo] == null)
        {
            stats[idTo] = stats[idFrom];
        }
        else
        {
            AllStats.Remove(stats[idFrom]);
            BlockMinedStats.Remove(stats[idFrom]);
            GeneralStats.Remove(stats[idFrom]);
            stats[idFrom] = stats[idTo];
        }
    }

    public static StatBase? GetStatById(int id) => IdToStat.GetValueOrDefault(id);
}