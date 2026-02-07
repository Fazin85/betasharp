using betareborn.Blocks;
using betareborn.Items;
using java.util;

namespace betareborn
{
    public class Achievements
    {
        public static int minColumn;
        public static int minRow;
        public static int maxColumn;
        public static int maxRow;
        public static List ACHIEVEMENTS = new ArrayList();
        public static Achievement OPEN_INVENTORY = (new Achievement(0, "openInventory", 0, 0, Item.book, (Achievement)null)).m_66876377().registerAchievement();
        public static Achievement MINE_WOOD = (new Achievement(1, "mineWood", 2, 1, Block.LOG, OPEN_INVENTORY)).registerAchievement();
        public static Achievement BUILD_WORKBENCH = (new Achievement(2, "buildWorkBench", 4, -1, Block.CRAFTING_TABLE, MINE_WOOD)).registerAchievement();
        public static Achievement BUILD_PICKAXE = (new Achievement(3, "buildPickaxe", 4, 2, Item.pickaxeWood, BUILD_WORKBENCH)).registerAchievement();
        public static Achievement BUILD_FURNACE = (new Achievement(4, "buildFurnace", 3, 4, Block.LIT_FURNACE, BUILD_PICKAXE)).registerAchievement();
        public static Achievement ACQUIRE_IRON = (new Achievement(5, "acquireIron", 1, 4, Item.ingotIron, BUILD_FURNACE)).registerAchievement();
        public static Achievement BUILD_HOE = (new Achievement(6, "buildHoe", 2, -3, Item.hoeWood, BUILD_WORKBENCH)).registerAchievement();
        public static Achievement MAKE_BREAD = (new Achievement(7, "makeBread", -1, -3, Item.bread, BUILD_HOE)).registerAchievement();
        public static Achievement BAKE_CAKE = (new Achievement(8, "bakeCake", 0, -5, Item.cake, BUILD_HOE)).registerAchievement();
        public static Achievement CRAFT_STONE_PICKAXE = (new Achievement(9, "buildBetterPickaxe", 6, 2, Item.pickaxeStone, BUILD_PICKAXE)).registerAchievement();
        public static Achievement COOK_FISH = (new Achievement(10, "cookFish", 2, 6, Item.fishCooked, BUILD_FURNACE)).registerAchievement();
        public static Achievement CRAFT_RAIL = (new Achievement(11, "onARail", 2, 3, Block.RAIL, ACQUIRE_IRON)).challenge().registerAchievement();
        public static Achievement CRAFT_SWORD = (new Achievement(12, "buildSword", 6, -1, Item.swordWood, BUILD_WORKBENCH)).registerAchievement();
        public static Achievement KILL_ENEMY = (new Achievement(13, "killEnemy", 8, -1, Item.bone, CRAFT_SWORD)).registerAchievement();
        public static Achievement KILL_COW = (new Achievement(14, "killCow", 7, -3, Item.leather, CRAFT_SWORD)).registerAchievement();
        public static Achievement KILL_PIG = (new Achievement(15, "flyPig", 8, -4, Item.saddle, KILL_COW)).challenge().registerAchievement();

        public static void initialize()
        {
        }

        static Achievements()
        {
            java.lang.System.@out.println(ACHIEVEMENTS.size() + " achievements");
        }
    }

}