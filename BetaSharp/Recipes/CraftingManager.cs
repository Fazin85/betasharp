using BetaSharp.Blocks;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using java.util;

namespace BetaSharp.Recipes;

public class CraftingManager
{
    private static readonly CraftingManager instance = new CraftingManager();
    private List recipes = new ArrayList();

    public static CraftingManager getInstance()
    {
        return instance;
    }

    private CraftingManager()
    {
        new RecipesTools().addRecipes(this);
        new RecipesWeapons().addRecipes(this);
        new RecipesIngots().addRecipes(this);
        new RecipesFood().addRecipes(this);
        new RecipesCrafting().addRecipes(this);
        new RecipesArmor().addRecipes(this);
        new RecipesDyes().addRecipes(this);
        addRecipe(new ItemStack(Item.PAPER, 3), ["###", java.lang.Character.valueOf('#'), Item.SUGAR_CANE]);
        addRecipe(new ItemStack(Item.BOOK, 1), ["#", "#", "#", java.lang.Character.valueOf('#'), Item.PAPER]);
        addRecipe(new ItemStack(Block.FENCE, 2), ["###", "###", java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.JUKEBOX, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.Planks, java.lang.Character.valueOf('X'), Item.DIAMOND]);
        addRecipe(new ItemStack(Block.Noteblock, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.Planks, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Block.Bookshelf, 1), ["###", "XXX", "###", java.lang.Character.valueOf('#'), Block.Planks, java.lang.Character.valueOf('X'), Item.BOOK]);
        addRecipe(new ItemStack(Block.SnowBlock, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.SNOWBALL]);
        addRecipe(new ItemStack(Block.Clay, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.CLAY]);
        addRecipe(new ItemStack(Block.Bricks, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.BRICK]);
        addRecipe(new ItemStack(Block.Glowstone, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.GLOWSTONE_DUST]);
        addRecipe(new ItemStack(Block.Wool, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.STRING]);
        addRecipe(new ItemStack(Block.TNT, 1), ["X#X", "#X#", "X#X", java.lang.Character.valueOf('X'), Item.GUNPOWDER, java.lang.Character.valueOf('#'), Block.SAND]);
        addRecipe(new ItemStack(Block.Slab., 3, 3), ["###", java.lang.Character.valueOf('#'), Block.Cobblestone]);
        addRecipe(new ItemStack(Block.Slab., 3, 0), ["###", java.lang.Character.valueOf('#'), Block.Stone]);
        addRecipe(new ItemStack(Block.Slab., 3, 1), ["###", java.lang.Character.valueOf('#'), Block.Sandstone]);
        addRecipe(new ItemStack(Block.Slab., 3, 2), ["###", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Block.Ladder, 2), ["# #", "###", "# #", java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Item.WOODEN_DOOR, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Block.Trapdoor, 2), ["###", "###", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Item.IRON_DOOR, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        addRecipe(new ItemStack(Item.SIGN, 1), ["###", "###", " X ", java.lang.Character.valueOf('#'), Block.Planks, java.lang.Character.valueOf('X'), Item.STICK]);
        addRecipe(new ItemStack(Item.CAKE, 1), ["AAA", "BEB", "CCC", java.lang.Character.valueOf('A'), Item.MILK_BUCKET, java.lang.Character.valueOf('B'), Item.SUGAR, java.lang.Character.valueOf('C'), Item.WHEAT, java.lang.Character.valueOf('E'), Item.EGG]);
        addRecipe(new ItemStack(Item.SUGAR, 1), ["#", java.lang.Character.valueOf('#'), Item.SUGAR_CANE]);
        addRecipe(new ItemStack(Block.Planks, 4), ["#", java.lang.Character.valueOf('#'), Block.Log]);
        addRecipe(new ItemStack(Item.STICK, 4), ["#", "#", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Block.Torch, 4), ["X", "#", java.lang.Character.valueOf('X'), Item.COAL, java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.Torch, 4), ["X", "#", java.lang.Character.valueOf('X'), new ItemStack(Item.COAL, 1, 1), java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Item.BOWL, 4), ["# #", " # ", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Block.Rail, 16), ["X X", "X#X", "X X", java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.PoweredRail, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.GOLD_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.DetectorRail, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('#'), Block.Stone_PRESSURE_PLATE]);
        addRecipe(new ItemStack(Item.MINECART, 1), ["# #", "###", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        addRecipe(new ItemStack(Block.JackLantern, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.Pumpkin, java.lang.Character.valueOf('B'), Block.Torch]);
        addRecipe(new ItemStack(Item.CHEST_MINECART, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.Chest, java.lang.Character.valueOf('B'), Item.MINECART]);
        addRecipe(new ItemStack(Item.FURNACE_MINECART, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.Furnace, java.lang.Character.valueOf('B'), Item.MINECART]);
        addRecipe(new ItemStack(Item.BOAT, 1), ["# #", "###", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Item.BUCKET, 1), ["# #", " # ", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        addRecipe(new ItemStack(Item.FLINT_AND_STEEL, 1), ["A ", " B", java.lang.Character.valueOf('A'), Item.IRON_INGOT, java.lang.Character.valueOf('B'), Item.FLINT]);
        addRecipe(new ItemStack(Item.BREAD, 1), ["###", java.lang.Character.valueOf('#'), Item.WHEAT]);
        addRecipe(new ItemStack(Block.WoodenStairs, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Item.FISHING_ROD, 1), ["  #", " #X", "# X", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Item.STRING]);
        addRecipe(new ItemStack(Block.CobblestoneStairs, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.Cobblestone]);
        addRecipe(new ItemStack(Item.PAINTING, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Block.Wool]);
        addRecipe(new ItemStack(Item.GOLDEN_APPLE, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.GoldBlock, java.lang.Character.valueOf('X'), Item.APPLE]);
        addRecipe(new ItemStack(Block.Lever, 1), ["X", "#", java.lang.Character.valueOf('#'), Block.Cobblestone, java.lang.Character.valueOf('X'), Item.STICK]);
        addRecipe(new ItemStack(Block.LitRedstoneTorch, 1), ["X", "#", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Item.REPEATER, 1), ["#X#", "III", java.lang.Character.valueOf('#'), Block.LitRedstoneTorch, java.lang.Character.valueOf('X'), Item.REDSTONE, java.lang.Character.valueOf('I'), Block.Stone]);
        addRecipe(new ItemStack(Item.CLOCK, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.GOLD_INGOT, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Item.COMPASS, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.IRON_INGOT, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Item.MAP, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.PAPER, java.lang.Character.valueOf('X'), Item.COMPASS]);
        addRecipe(new ItemStack(Block.Button, 1), ["#", "#", java.lang.Character.valueOf('#'), Block.Stone]);
        addRecipe(new ItemStack(Block.Stone_PRESSURE_PLATE, 1), ["##", java.lang.Character.valueOf('#'), Block.Stone]);
        addRecipe(new ItemStack(Block.WoodenPressurePlate, 1), ["##", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Block.Dispenser, 1), ["###", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.Cobblestone, java.lang.Character.valueOf('X'), Item.BOW, java.lang.Character.valueOf('R'), Item.REDSTONE]);
        addRecipe(new ItemStack(Block.PISTON, 1), ["TTT", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.Cobblestone, java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('T'), Block.Planks]);
        addRecipe(new ItemStack(Block.StickyPiston, 1), ["S", "P", java.lang.Character.valueOf('S'), Item.SLIMEBALL, java.lang.Character.valueOf('P'), Block.PISTON]);
        addRecipe(new ItemStack(Item.BED, 1), ["###", "XXX", java.lang.Character.valueOf('#'), Block.Wool, java.lang.Character.valueOf('X'), Block.Planks]);
        Collections.sort(recipes, new RecipeSorter());
        java.lang.System.@out.println(recipes.size() + " recipes");
    }

    public void addRecipe(ItemStack var1, params object[] var2)
    {
        string var3 = "";
        int var4 = 0;
        int var5 = 0;
        int var6 = 0;
        if (var2[var4] is string[])
        {
            string[] var11 = (string[])var2[var4++];

            for (int var8 = 0; var8 < var11.Length; ++var8)
            {
                string var9 = var11[var8];
                ++var6;
                var5 = var9.Length;
                var3 = var3 + var9;
            }
        }
        else
        {
            while (var2[var4] is string)
            {
                string var7 = (string)var2[var4++];
                ++var6;
                var5 = var7.Length;
                var3 = var3 + var7;
            }
        }

        HashMap var12;
        for (var12 = new HashMap(); var4 < var2.Length; var4 += 2)
        {
            java.lang.Character var13 = (java.lang.Character)var2[var4];
            ItemStack var15 = null;
            if (var2[var4 + 1] is Item)
            {
                var15 = new ItemStack((Item)var2[var4 + 1]);
            }
            else if (var2[var4 + 1] is Block)
            {
                var15 = new ItemStack((Block)var2[var4 + 1], 1, -1);
            }
            else if (var2[var4 + 1] is ItemStack)
            {
                var15 = (ItemStack)var2[var4 + 1];
            }

            var12.put(var13, var15);
        }

        ItemStack[] var14 = new ItemStack[var5 * var6];

        for (int var16 = 0; var16 < var5 * var6; ++var16)
        {
            char var10 = var3[var16];
            if (var12.containsKey(java.lang.Character.valueOf(var10)))
            {
                var14[var16] = ((ItemStack)var12.get(java.lang.Character.valueOf(var10))).copy();
            }
            else
            {
                var14[var16] = null;
            }
        }

        recipes.add(new ShapedRecipes(var5, var6, var14, var1));
    }

    public void addShapelessRecipe(ItemStack var1, params object[] var2)
    {
        ArrayList var3 = new ArrayList();
        object[] var4 = var2;
        int var5 = var2.Length;

        for (int var6 = 0; var6 < var5; ++var6)
        {
            object var7 = var4[var6];
            if (var7 is ItemStack)
            {
                var3.add(((ItemStack)var7).copy());
            }
            else if (var7 is Item)
            {
                var3.add(new ItemStack((Item)var7));
            }
            else
            {
                if (!(var7 is Block))
                {
                    throw new java.lang.RuntimeException("Invalid shapeless recipy!");
                }

                var3.add(new ItemStack((Block)var7));
            }
        }

        recipes.add(new ShapelessRecipes(var1, var3));
    }

    public ItemStack findMatchingRecipe(InventoryCrafting var1)
    {
        for (int var2 = 0; var2 < recipes.size(); ++var2)
        {
            IRecipe var3 = (IRecipe)recipes.get(var2);
            if (var3.matches(var1))
            {
                return var3.getCraftingResult(var1);
            }
        }

        return null;
    }

    public List getRecipeList()
    {
        return recipes;
    }
}