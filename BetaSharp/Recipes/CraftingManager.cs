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
        new RecipesTools().AddRecipes(this);
        new RecipesWeapons().AddRecipes(this);
        new RecipesIngots().AddRecipes(this);
        new RecipesFood().AddRecipes(this);
        new RecipesCrafting().AddRecipes(this);
        new RecipesArmor().AddRecipes(this);
        new RecipesDyes().AddRecipes(this);
        AddRecipe(new ItemStack(Item.PAPER, 3), ["###", java.lang.Character.valueOf('#'), Item.SUGAR_CANE]);
        AddRecipe(new ItemStack(Item.BOOK, 1), ["#", "#", "#", java.lang.Character.valueOf('#'), Item.PAPER]);
        AddRecipe(new ItemStack(Block.FENCE, 2), ["###", "###", java.lang.Character.valueOf('#'), Item.STICK]);
        AddRecipe(new ItemStack(Block.JUKEBOX, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.DIAMOND]);
        AddRecipe(new ItemStack(Block.NOTE_BLOCK, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        AddRecipe(new ItemStack(Block.BOOKSHELF, 1), ["###", "XXX", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.BOOK]);
        AddRecipe(new ItemStack(Block.SNOW_BLOCK, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.SNOWBALL]);
        AddRecipe(new ItemStack(Block.CLAY, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.CLAY]);
        AddRecipe(new ItemStack(Block.BRICKS, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.BRICK]);
        AddRecipe(new ItemStack(Block.GLOWSTONE, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.GLOWSTONE_DUST]);
        AddRecipe(new ItemStack(Block.WOOL, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.STRING]);
        AddRecipe(new ItemStack(Block.TNT, 1), ["X#X", "#X#", "X#X", java.lang.Character.valueOf('X'), Item.GUNPOWDER, java.lang.Character.valueOf('#'), Block.SAND]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 3), ["###", java.lang.Character.valueOf('#'), Block.COBBLESTONE]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 0), ["###", java.lang.Character.valueOf('#'), Block.STONE]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 1), ["###", java.lang.Character.valueOf('#'), Block.SANDSTONE]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 2), ["###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Block.LADDER, 2), ["# #", "###", "# #", java.lang.Character.valueOf('#'), Item.STICK]);
        AddRecipe(new ItemStack(Item.WOODEN_DOOR, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Block.TRAPDOOR, 2), ["###", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Item.IRON_DOOR, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        AddRecipe(new ItemStack(Item.SIGN, 1), ["###", "###", " X ", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.STICK]);
        AddRecipe(new ItemStack(Item.CAKE, 1), ["AAA", "BEB", "CCC", java.lang.Character.valueOf('A'), Item.MILK_BUCKET, java.lang.Character.valueOf('B'), Item.SUGAR, java.lang.Character.valueOf('C'), Item.WHEAT, java.lang.Character.valueOf('E'), Item.EGG]);
        AddRecipe(new ItemStack(Item.SUGAR, 1), ["#", java.lang.Character.valueOf('#'), Item.SUGAR_CANE]);
        AddRecipe(new ItemStack(Block.PLANKS, 4), ["#", java.lang.Character.valueOf('#'), Block.LOG]);
        AddRecipe(new ItemStack(Item.STICK, 4), ["#", "#", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", java.lang.Character.valueOf('X'), Item.COAL, java.lang.Character.valueOf('#'), Item.STICK]);
        AddRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", java.lang.Character.valueOf('X'), new ItemStack(Item.COAL, 1, 1), java.lang.Character.valueOf('#'), Item.STICK]);
        AddRecipe(new ItemStack(Item.BOWL, 4), ["# #", " # ", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Block.RAIL, 16), ["X X", "X#X", "X X", java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('#'), Item.STICK]);
        AddRecipe(new ItemStack(Block.POWERED_RAIL, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.GOLD_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('#'), Item.STICK]);
        AddRecipe(new ItemStack(Block.DETECTOR_RAIL, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('#'), Block.STONE_PRESSURE_PLATE]);
        AddRecipe(new ItemStack(Item.MINECART, 1), ["# #", "###", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        AddRecipe(new ItemStack(Block.JACK_O_LANTERN, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.PUMPKIN, java.lang.Character.valueOf('B'), Block.TORCH]);
        AddRecipe(new ItemStack(Item.CHEST_MINECART, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.CHEST, java.lang.Character.valueOf('B'), Item.MINECART]);
        AddRecipe(new ItemStack(Item.FURNACE_MINECART, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.FURNACE, java.lang.Character.valueOf('B'), Item.MINECART]);
        AddRecipe(new ItemStack(Item.BOAT, 1), ["# #", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Item.BUCKET, 1), ["# #", " # ", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        AddRecipe(new ItemStack(Item.FLINT_AND_STEEL, 1), ["A ", " B", java.lang.Character.valueOf('A'), Item.IRON_INGOT, java.lang.Character.valueOf('B'), Item.FLINT]);
        AddRecipe(new ItemStack(Item.BREAD, 1), ["###", java.lang.Character.valueOf('#'), Item.WHEAT]);
        AddRecipe(new ItemStack(Block.WOODEN_STAIRS, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Item.FISHING_ROD, 1), ["  #", " #X", "# X", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Item.STRING]);
        AddRecipe(new ItemStack(Block.COBBLESTONE_STAIRS, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.COBBLESTONE]);
        AddRecipe(new ItemStack(Item.PAINTING, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Block.WOOL]);
        AddRecipe(new ItemStack(Item.GOLDEN_APPLE, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.GOLD_BLOCK, java.lang.Character.valueOf('X'), Item.APPLE]);
        AddRecipe(new ItemStack(Block.LEVER, 1), ["X", "#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.STICK]);
        AddRecipe(new ItemStack(Block.LIT_REDSTONE_TORCH, 1), ["X", "#", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        AddRecipe(new ItemStack(Item.REPEATER, 1), ["#X#", "III", java.lang.Character.valueOf('#'), Block.LIT_REDSTONE_TORCH, java.lang.Character.valueOf('X'), Item.REDSTONE, java.lang.Character.valueOf('I'), Block.STONE]);
        AddRecipe(new ItemStack(Item.CLOCK, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.GOLD_INGOT, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        AddRecipe(new ItemStack(Item.COMPASS, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.IRON_INGOT, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        AddRecipe(new ItemStack(Item.MAP, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.PAPER, java.lang.Character.valueOf('X'), Item.COMPASS]);
        AddRecipe(new ItemStack(Block.BUTTON, 1), ["#", "#", java.lang.Character.valueOf('#'), Block.STONE]);
        AddRecipe(new ItemStack(Block.STONE_PRESSURE_PLATE, 1), ["##", java.lang.Character.valueOf('#'), Block.STONE]);
        AddRecipe(new ItemStack(Block.WOODEN_PRESSURE_PLATE, 1), ["##", java.lang.Character.valueOf('#'), Block.PLANKS]);
        AddRecipe(new ItemStack(Block.DISPENSER, 1), ["###", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.BOW, java.lang.Character.valueOf('R'), Item.REDSTONE]);
        AddRecipe(new ItemStack(Block.PISTON, 1), ["TTT", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('T'), Block.PLANKS]);
        AddRecipe(new ItemStack(Block.STICKY_PISTON, 1), ["S", "P", java.lang.Character.valueOf('S'), Item.SLIMEBALL, java.lang.Character.valueOf('P'), Block.PISTON]);
        AddRecipe(new ItemStack(Item.BED, 1), ["###", "XXX", java.lang.Character.valueOf('#'), Block.WOOL, java.lang.Character.valueOf('X'), Block.PLANKS]);
        Collections.sort(recipes, new RecipeSorter());
        java.lang.System.@out.println($"{recipes.size()} recipes");
    }

    public void AddRecipe(ItemStack result, params object[] pattern)
    {
        string patternString = "";
        int index = 0;
        int width = 0;
        int height = 0;
        if (pattern[index] is string[])
        {
            string[] rows = (string[])pattern[index++];

            for (int i = 0; i < rows.Length; ++i)
            {
                string row = rows[i];
                ++height;
                width = row.Length;
                patternString = patternString + row;
            }
        }
        else
        {
            while (pattern[index] is string)
            {
                string row = (string)pattern[index++];
                ++height;
                width = row.Length;
                patternString = patternString + row;
            }
        }

        HashMap ingredient;
        for (ingredient = new HashMap(); index < pattern.Length; index += 2)
        {
            java.lang.Character key = (java.lang.Character)pattern[index];
            ItemStack value = null;
            if (pattern[index + 1] is Item)
            {
                value = new ItemStack((Item)pattern[index + 1]);
            }
            else if (pattern[index + 1] is Block)
            {
                value = new ItemStack((Block)pattern[index + 1], 1, -1);
            }
            else if (pattern[index + 1] is ItemStack)
            {
                value = (ItemStack)pattern[index + 1];
            }

            ingredient.put(key, value);
        }

        ItemStack[] ingredientGrid = new ItemStack[width * height];

        for (int i = 0; i < width * height; ++i)
        {
            char c = patternString[i];
            if (ingredient.containsKey(java.lang.Character.valueOf(c)))
            {
                ingredientGrid[i] = ((ItemStack)ingredient.get(java.lang.Character.valueOf(c))).copy();
            }
            else
            {
                ingredientGrid[i] = null;
            }
        }

        recipes.add(new ShapedRecipes(width, height, ingredientGrid, result));
    }

    public void AddShapelessRecipe(ItemStack result, params object[] pattern)
    {
        ArrayList stacks = new ArrayList();
        int length = pattern.Length;

        for (int i = 0; i < length; ++i)
        {
            object var7 = pattern[i];
            if (var7 is ItemStack)
            {
                stacks.add(((ItemStack)var7).copy());
            }
            else if (var7 is Item)
            {
                stacks.add(new ItemStack((Item)var7));
            }
            else
            {
                if (!(var7 is Block))
                {
                    throw new java.lang.RuntimeException("Invalid shapeless recipy!");
                }
                stacks.add(new ItemStack((Block)var7));
            }
        }

        recipes.add(new ShapelessRecipes(result, stacks));
    }

    public ItemStack FindMatchingRecipe(InventoryCrafting craftingInventory)
    {
        for (int i = 0; i < recipes.size(); ++i)
        {
            IRecipe recipe = (IRecipe)recipes.get(i);
            if (recipe.Matches(craftingInventory))

                return recipe.GetCraftingResult(craftingInventory);
        }

        return null;
    }

    public List GetRecipeList()
    {
        return recipes;
    }
}