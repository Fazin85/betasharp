using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp;

public class Achievements
{
    public static int minColumn;
    public static int minRow;
    public static int maxColumn;
    public static int maxRow;

    public readonly static List<Achievement> AllAchievements = [];
    public readonly static Achievement OpenInventory = new Achievement(0, "openInventory", 0, 0, Item.Book, null).m_66876377().registerAchievement();
    public readonly static Achievement MineWood = new Achievement(1, "mineWood", 2, 1, Block.Log, OpenInventory).registerAchievement();
    public readonly static Achievement BuildWorkbench = new Achievement(2, "buildWorkBench", 4, -1, Block.CraftingTable, MineWood).registerAchievement();
    public readonly static Achievement BuildPickaxe = new Achievement(3, "buildPickaxe", 4, 2, Item.WoodenPickaxe, BuildWorkbench).registerAchievement();
    public readonly static Achievement BuildFurnace = new Achievement(4, "buildFurnace", 3, 4, Block.LitFurnace, BuildPickaxe).registerAchievement();
    public readonly static Achievement AcquireIron = new Achievement(5, "acquireIron", 1, 4, Item.IronIngot, BuildFurnace).registerAchievement();
    public readonly static Achievement BuildHoe = new Achievement(6, "buildHoe", 2, -3, Item.WoodenHoe, BuildWorkbench).registerAchievement();
    public readonly static Achievement MakeBread = new Achievement(7, "makeBread", -1, -3, Item.Bread, BuildHoe).registerAchievement();
    public readonly static Achievement MakeCake = new Achievement(8, "bakeCake", 0, -5, Item.Cake, BuildHoe).registerAchievement();
    public readonly static Achievement CraftStonePickaxe = new Achievement(9, "buildBetterPickaxe", 6, 2, Item.StonePickaxe, BuildPickaxe).registerAchievement();
    public readonly static Achievement CookFish = new Achievement(10, "cookFish", 2, 6, Item.CookedFish, BuildFurnace).registerAchievement();
    public readonly static Achievement CraftRail = new Achievement(11, "onARail", 2, 3, Block.Rail, AcquireIron).challenge().registerAchievement();
    public readonly static Achievement CraftSword = new Achievement(12, "buildSword", 6, -1, Item.WoodenSword, BuildWorkbench).registerAchievement();
    public readonly static Achievement KillEnemy = new Achievement(13, "killEnemy", 8, -1, Item.Bone, CraftSword).registerAchievement();
    public readonly static Achievement KillCow = new Achievement(14, "killCow", 7, -3, Item.Leather, CraftSword).registerAchievement();
    public readonly static Achievement KillPig = new Achievement(15, "flyPig", 8, -4, Item.Saddle, KillCow).challenge().registerAchievement();

    public static void initialize()
    {
    }

    static Achievements()
    {
        Log.Info($"{AllAchievements.Count} achievements");
    }
}
