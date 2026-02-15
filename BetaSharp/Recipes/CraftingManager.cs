using BetaSharp.Blocks;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using java.util;

namespace BetaSharp.Recipes;

public class CraftingManager
{
    private static CraftingManager instance { get; } = new();
    private List<IRecipe> _recipes = new();
    public List<IRecipe> Recipes => _recipes;

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
        addRecipe(new ItemStack(Block.Fence, 2), ["###", "###", java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.Jukebox, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.Planks, java.lang.Character.valueOf('X'), Item.DIAMOND]);
        addRecipe(new ItemStack(Block.Noteblock, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.Planks, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Block.Bookshelf, 1), ["###", "XXX", "###", java.lang.Character.valueOf('#'), Block.Planks, java.lang.Character.valueOf('X'), Item.BOOK]);
        addRecipe(new ItemStack(Block.SnowBlock, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.SNOWBALL]);
        addRecipe(new ItemStack(Block.Clay, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.CLAY]);
        addRecipe(new ItemStack(Block.Bricks, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.BRICK]);
        addRecipe(new ItemStack(Block.Glowstone, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.GLOWSTONE_DUST]);
        addRecipe(new ItemStack(Block.Wool, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.STRING]);
        addRecipe(new ItemStack(Block.TNT, 1), ["X#X", "#X#", "X#X", java.lang.Character.valueOf('X'), Item.GUNPOWDER, java.lang.Character.valueOf('#'), Block.Sand]);
        addRecipe(new ItemStack(Block.Slab, 3, 3), ["###", java.lang.Character.valueOf('#'), Block.Cobblestone]);
        addRecipe(new ItemStack(Block.Slab, 3, 0), ["###", java.lang.Character.valueOf('#'), Block.Stone]);
        addRecipe(new ItemStack(Block.Slab, 3, 1), ["###", java.lang.Character.valueOf('#'), Block.Sandstone]);
        addRecipe(new ItemStack(Block.Slab, 3, 2), ["###", java.lang.Character.valueOf('#'), Block.Planks]);
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
        addRecipe(new ItemStack(Block.DetectorRail, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('#'), Block.StonePressurePlate]);
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
        addRecipe(new ItemStack(Block.StonePressurePlate, 1), ["##", java.lang.Character.valueOf('#'), Block.Stone]);
        addRecipe(new ItemStack(Block.WoodenPressurePlate, 1), ["##", java.lang.Character.valueOf('#'), Block.Planks]);
        addRecipe(new ItemStack(Block.Dispenser, 1), ["###", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.Cobblestone, java.lang.Character.valueOf('X'), Item.BOW, java.lang.Character.valueOf('R'), Item.REDSTONE]);
        addRecipe(new ItemStack(Block.Piston, 1), ["TTT", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.Cobblestone, java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('T'), Block.Planks]);
        addRecipe(new ItemStack(Block.StickyPiston, 1), ["S", "P", java.lang.Character.valueOf('S'), Item.SLIMEBALL, java.lang.Character.valueOf('P'), Block.Piston]);
        addRecipe(new ItemStack(Item.BED, 1), ["###", "XXX", java.lang.Character.valueOf('#'), Block.Wool, java.lang.Character.valueOf('X'), Block.Planks]);
        Collections.sort(recipes, new RecipeSorter());
        java.lang.System.@out.println(recipes.size() + " recipes");
    }

    public void AddRecipe(ItemStack result, params object[] pattern)
    {
        string patternString = "";
        int index = 0;
        int width = 0;
        int height = 0;

        while (index < pattern.Length && (pattern[index] is string || pattern[index] is string[]))
        {
            object current = pattern[index++];
            if (current is string[] rows)
            {
                foreach (var row in rows)
                {
                    height++;
                    width = row.Length;
                    patternString += row;
                }
            }
            else if (current is string row)
            {
                height++;
                width = row.Length;
                patternString += row;
            }
        }

        var ingredients = new Dictionary<char, ItemStack?>();
        for (; index < pattern.Length; index += 2)
        {
            char key = (char)pattern[index];
            object input = pattern[index + 1];

            ItemStack? value = input switch
            {
                Item item       => new ItemStack(item),
                Block block     => new ItemStack(block, 1, -1),
                ItemStack stack => stack,
                _               => null // Thowing some Exception here would be ideal, but the original game does not do this
            };

            ingredients[key] = value;
        }

        var ingredientGrid = new ItemStack?[width * height];

        for (int i = 0; i < patternString.Length; i++)
        {
            char c = patternString[i];
            ingredients.TryGetValue(c, out var stack);
            ingredientGrid[i] = stack?.copy() ?? null;
        }

        _recipes.Add(new ShapedRecipes(width, height, ingredientGrid, result));
    }

    public void AddShapelessRecipe(ItemStack result, params object[] pattern)
    {
        List<ItemStack> stacks = new();

        foreach (var ingredient in pattern)
        {
            switch (ingredient)
            {
                case ItemStack s: stacks.Add(s.copy()); break;
                case Item i: stacks.Add(new ItemStack(i)); break;
                case Block b: stacks.Add(new ItemStack(b)); break;
                default:
                    throw new InvalidOperationException("Invalid shapeless recipy!"); // This typo is intentional to match the original game
            }
        }

        _recipes.Add(new ShapelessRecipes(result, stacks));
    }

    public ItemStack? FindMatchingRecipe(InventoryCrafting craftingInventory)
    {
        return _recipes
            .FirstOrDefault(r => r.Matches(craftingInventory))
            ?.GetCraftingResult(craftingInventory);
    }
}