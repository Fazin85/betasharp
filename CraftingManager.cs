using betareborn.Blocks;
using betareborn.Items;
using betareborn.Recipes;
using java.util;

namespace betareborn
{
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
            (new RecipesTools()).addRecipes(this);
            (new RecipesWeapons()).addRecipes(this);
            (new RecipesIngots()).addRecipes(this);
            (new RecipesFood()).addRecipes(this);
            (new RecipesCrafting()).addRecipes(this);
            (new RecipesArmor()).addRecipes(this);
            (new RecipesDyes()).addRecipes(this);
            addRecipe(new ItemStack(Item.paper, 3), ["###", java.lang.Character.valueOf('#'), Item.reed]);
            addRecipe(new ItemStack(Item.book, 1), ["#", "#", "#", java.lang.Character.valueOf('#'), Item.paper]);
            addRecipe(new ItemStack(Block.FENCE, 2), ["###", "###", java.lang.Character.valueOf('#'), Item.stick]);
            addRecipe(new ItemStack(Block.JUKEBOX, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.diamond]);
            addRecipe(new ItemStack(Block.NOTE_BLOCK, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.redstone]);
            addRecipe(new ItemStack(Block.BOOKSHELF, 1), ["###", "XXX", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.book]);
            addRecipe(new ItemStack(Block.SNOW_BLOCK, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.snowball]);
            addRecipe(new ItemStack(Block.CLAY, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.clay]);
            addRecipe(new ItemStack(Block.BRICKS, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.brick]);
            addRecipe(new ItemStack(Block.GLOWSTONE, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.lightStoneDust]);
            addRecipe(new ItemStack(Block.WOOL, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.silk]);
            addRecipe(new ItemStack(Block.TNT, 1), ["X#X", "#X#", "X#X", java.lang.Character.valueOf('X'), Item.gunpowder, java.lang.Character.valueOf('#'), Block.SAND]);
            addRecipe(new ItemStack(Block.SLAB, 3, 3), ["###", java.lang.Character.valueOf('#'), Block.COBBLESTONE]);
            addRecipe(new ItemStack(Block.SLAB, 3, 0), ["###", java.lang.Character.valueOf('#'), Block.STONE]);
            addRecipe(new ItemStack(Block.SLAB, 3, 1), ["###", java.lang.Character.valueOf('#'), Block.SANDSTONE]);
            addRecipe(new ItemStack(Block.SLAB, 3, 2), ["###", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Block.LADDER, 2), ["# #", "###", "# #", java.lang.Character.valueOf('#'), Item.stick]);
            addRecipe(new ItemStack(Item.doorWood, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Block.TRAPDOOR, 2), ["###", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Item.doorSteel, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Item.ingotIron]);
            addRecipe(new ItemStack(Item.sign, 1), ["###", "###", " X ", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.stick]);
            addRecipe(new ItemStack(Item.cake, 1), ["AAA", "BEB", "CCC", java.lang.Character.valueOf('A'), Item.bucketMilk, java.lang.Character.valueOf('B'), Item.sugar, java.lang.Character.valueOf('C'), Item.wheat, java.lang.Character.valueOf('E'), Item.egg]);
            addRecipe(new ItemStack(Item.sugar, 1), ["#", java.lang.Character.valueOf('#'), Item.reed]);
            addRecipe(new ItemStack(Block.PLANKS, 4), ["#", java.lang.Character.valueOf('#'), Block.LOG]);
            addRecipe(new ItemStack(Item.stick, 4), ["#", "#", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", java.lang.Character.valueOf('X'), Item.coal, java.lang.Character.valueOf('#'), Item.stick]);
            addRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", java.lang.Character.valueOf('X'), new ItemStack(Item.coal, 1, 1), java.lang.Character.valueOf('#'), Item.stick]);
            addRecipe(new ItemStack(Item.bowlEmpty, 4), ["# #", " # ", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Block.RAIL, 16), ["X X", "X#X", "X X", java.lang.Character.valueOf('X'), Item.ingotIron, java.lang.Character.valueOf('#'), Item.stick]);
            addRecipe(new ItemStack(Block.POWERED_RAIL, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.ingotGold, java.lang.Character.valueOf('R'), Item.redstone, java.lang.Character.valueOf('#'), Item.stick]);
            addRecipe(new ItemStack(Block.DETECTOR_RAIL, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.ingotIron, java.lang.Character.valueOf('R'), Item.redstone, java.lang.Character.valueOf('#'), Block.STONE_PRESSURE_PLATE]);
            addRecipe(new ItemStack(Item.minecartEmpty, 1), ["# #", "###", java.lang.Character.valueOf('#'), Item.ingotIron]);
            addRecipe(new ItemStack(Block.JACK_O_LANTERN, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.PUMPKIN, java.lang.Character.valueOf('B'), Block.TORCH]);
            addRecipe(new ItemStack(Item.minecartCrate, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.CHEST, java.lang.Character.valueOf('B'), Item.minecartEmpty]);
            addRecipe(new ItemStack(Item.minecartPowered, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.FURNACE, java.lang.Character.valueOf('B'), Item.minecartEmpty]);
            addRecipe(new ItemStack(Item.boat, 1), ["# #", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Item.bucketEmpty, 1), ["# #", " # ", java.lang.Character.valueOf('#'), Item.ingotIron]);
            addRecipe(new ItemStack(Item.flintAndSteel, 1), ["A ", " B", java.lang.Character.valueOf('A'), Item.ingotIron, java.lang.Character.valueOf('B'), Item.flint]);
            addRecipe(new ItemStack(Item.bread, 1), ["###", java.lang.Character.valueOf('#'), Item.wheat]);
            addRecipe(new ItemStack(Block.WOODEN_STAIRS, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Item.fishingRod, 1), ["  #", " #X", "# X", java.lang.Character.valueOf('#'), Item.stick, java.lang.Character.valueOf('X'), Item.silk]);
            addRecipe(new ItemStack(Block.COBBLESTONE_STAIRS, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.COBBLESTONE]);
            addRecipe(new ItemStack(Item.painting, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.stick, java.lang.Character.valueOf('X'), Block.WOOL]);
            addRecipe(new ItemStack(Item.appleGold, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.GOLD_BLOCK, java.lang.Character.valueOf('X'), Item.appleRed]);
            addRecipe(new ItemStack(Block.LEVER, 1), ["X", "#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.stick]);
            addRecipe(new ItemStack(Block.LIT_REDSTONE_TORCH, 1), ["X", "#", java.lang.Character.valueOf('#'), Item.stick, java.lang.Character.valueOf('X'), Item.redstone]);
            addRecipe(new ItemStack(Item.redstoneRepeater, 1), ["#X#", "III", java.lang.Character.valueOf('#'), Block.LIT_REDSTONE_TORCH, java.lang.Character.valueOf('X'), Item.redstone, java.lang.Character.valueOf('I'), Block.STONE]);
            addRecipe(new ItemStack(Item.pocketSundial, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.ingotGold, java.lang.Character.valueOf('X'), Item.redstone]);
            addRecipe(new ItemStack(Item.compass, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.ingotIron, java.lang.Character.valueOf('X'), Item.redstone]);
            addRecipe(new ItemStack(Item.mapItem, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.paper, java.lang.Character.valueOf('X'), Item.compass]);
            addRecipe(new ItemStack(Block.BUTTON, 1), ["#", "#", java.lang.Character.valueOf('#'), Block.STONE]);
            addRecipe(new ItemStack(Block.STONE_PRESSURE_PLATE, 1), ["##", java.lang.Character.valueOf('#'), Block.STONE]);
            addRecipe(new ItemStack(Block.WOODEN_PRESSURE_PLATE, 1), ["##", java.lang.Character.valueOf('#'), Block.PLANKS]);
            addRecipe(new ItemStack(Block.DISPENSER, 1), ["###", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.bow, java.lang.Character.valueOf('R'), Item.redstone]);
            addRecipe(new ItemStack(Block.PISTON, 1), ["TTT", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.ingotIron, java.lang.Character.valueOf('R'), Item.redstone, java.lang.Character.valueOf('T'), Block.PLANKS]);
            addRecipe(new ItemStack(Block.STICKY_PISTON, 1), ["S", "P", java.lang.Character.valueOf('S'), Item.slimeBall, java.lang.Character.valueOf('P'), Block.PISTON]);
            addRecipe(new ItemStack(Item.bed, 1), ["###", "XXX", java.lang.Character.valueOf('#'), Block.WOOL, java.lang.Character.valueOf('X'), Block.PLANKS]);
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
                string[] var11 = (string[])((string[])var2[var4++]);

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

}