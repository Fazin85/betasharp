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
        new RecipesTools().AddRecipes(this);
        new RecipesWeapons().AddRecipes(this);
        new RecipesIngots().AddRecipes(this);
        new RecipesFood().AddRecipes(this);
        new RecipesCrafting().AddRecipes(this);
        new RecipesArmor().AddRecipes(this);
        new RecipesDyes().AddRecipes(this);

        AddRecipe(new ItemStack(Item.PAPER, 3), ["###", '#', Item.SUGAR_CANE]);
        AddRecipe(new ItemStack(Item.BOOK, 1), ["#", "#", "#", '#', Item.PAPER]);
        AddRecipe(new ItemStack(Block.FENCE, 2), ["###", "###", '#', Item.STICK]);
        AddRecipe(new ItemStack(Block.JUKEBOX, 1), ["###", "#X#", "###", '#', Block.PLANKS, 'X', Item.DIAMOND]);
        AddRecipe(new ItemStack(Block.NOTE_BLOCK, 1), ["###", "#X#", "###", '#', Block.PLANKS, 'X', Item.REDSTONE]);
        AddRecipe(new ItemStack(Block.BOOKSHELF, 1), ["###", "XXX", "###", '#', Block.PLANKS, 'X', Item.BOOK]);
        AddRecipe(new ItemStack(Block.SNOW_BLOCK, 1), ["##", "##", '#', Item.SNOWBALL]);
        AddRecipe(new ItemStack(Block.CLAY, 1), ["##", "##", '#', Item.CLAY]);
        AddRecipe(new ItemStack(Block.BRICKS, 1), ["##", "##", '#', Item.BRICK]);
        AddRecipe(new ItemStack(Block.GLOWSTONE, 1), ["##", "##", '#', Item.GLOWSTONE_DUST]);
        AddRecipe(new ItemStack(Block.WOOL, 1), ["##", "##", '#', Item.STRING]);
        AddRecipe(new ItemStack(Block.TNT, 1), ["X#X", "#X#", "X#X", 'X', Item.GUNPOWDER, '#', Block.SAND]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 3), ["###", '#', Block.COBBLESTONE]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 0), ["###", '#', Block.STONE]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 1), ["###", '#', Block.SANDSTONE]);
        AddRecipe(new ItemStack(Block.SLAB, 3, 2), ["###", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Block.LADDER, 2), ["# #", "###", "# #", '#', Item.STICK]);
        AddRecipe(new ItemStack(Item.WOODEN_DOOR, 1), ["##", "##", "##", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Block.TRAPDOOR, 2), ["###", "###", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Item.IRON_DOOR, 1), ["##", "##", "##", '#', Item.IRON_INGOT]);
        AddRecipe(new ItemStack(Item.SIGN, 1), ["###", "###", " X ", '#', Block.PLANKS, 'X', Item.STICK]);
        AddRecipe(new ItemStack(Item.CAKE, 1), ["AAA", "BEB", "CCC", 'A', Item.MILK_BUCKET, 'B', Item.SUGAR, 'C', Item.WHEAT, 'E', Item.EGG]);
        AddRecipe(new ItemStack(Item.SUGAR, 1), ["#", '#', Item.SUGAR_CANE]);
        AddRecipe(new ItemStack(Block.PLANKS, 4), ["#", '#', Block.LOG]);
        AddRecipe(new ItemStack(Item.STICK, 4), ["#", "#", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", 'X', Item.COAL, '#', Item.STICK]);
        AddRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", 'X', new ItemStack(Item.COAL, 1, 1), '#', Item.STICK]);
        AddRecipe(new ItemStack(Item.BOWL, 4), ["# #", " # ", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Block.RAIL, 16), ["X X", "X#X", "X X", 'X', Item.IRON_INGOT, '#', Item.STICK]);
        AddRecipe(new ItemStack(Block.POWERED_RAIL, 6), ["X X", "X#X", "XRX", 'X', Item.GOLD_INGOT, 'R', Item.REDSTONE, '#', Item.STICK]);
        AddRecipe(new ItemStack(Block.DETECTOR_RAIL, 6), ["X X", "X#X", "XRX", 'X', Item.IRON_INGOT, 'R', Item.REDSTONE, '#', Block.STONE_PRESSURE_PLATE]);
        AddRecipe(new ItemStack(Item.MINECART, 1), ["# #", "###", '#', Item.IRON_INGOT]);
        AddRecipe(new ItemStack(Block.JACK_O_LANTERN, 1), ["A", "B", 'A', Block.PUMPKIN, 'B', Block.TORCH]);
        AddRecipe(new ItemStack(Item.CHEST_MINECART, 1), ["A", "B", 'A', Block.CHEST, 'B', Item.MINECART]);
        AddRecipe(new ItemStack(Item.FURNACE_MINECART, 1), ["A", "B", 'A', Block.FURNACE, 'B', Item.MINECART]);
        AddRecipe(new ItemStack(Item.BOAT, 1), ["# #", "###", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Item.BUCKET, 1), ["# #", " # ", '#', Item.IRON_INGOT]);
        AddRecipe(new ItemStack(Item.FLINT_AND_STEEL, 1), ["A ", " B", 'A', Item.IRON_INGOT, 'B', Item.FLINT]);
        AddRecipe(new ItemStack(Item.BREAD, 1), ["###", '#', Item.WHEAT]);
        AddRecipe(new ItemStack(Block.WOODEN_STAIRS, 4), ["#  ", "## ", "###", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Item.FISHING_ROD, 1), ["  #", " #X", "# X", '#', Item.STICK, 'X', Item.STRING]);
        AddRecipe(new ItemStack(Block.COBBLESTONE_STAIRS, 4), ["#  ", "## ", "###", '#', Block.COBBLESTONE]);
        AddRecipe(new ItemStack(Item.PAINTING, 1), ["###", "#X#", "###", '#', Item.STICK, 'X', Block.WOOL]);
        AddRecipe(new ItemStack(Item.GOLDEN_APPLE, 1), ["###", "#X#", "###", '#', Block.GOLD_BLOCK, 'X', Item.APPLE]);
        AddRecipe(new ItemStack(Block.LEVER, 1), ["X", "#", '#', Block.COBBLESTONE, 'X', Item.STICK]);
        AddRecipe(new ItemStack(Block.LIT_REDSTONE_TORCH, 1), ["X", "#", '#', Item.STICK, 'X', Item.REDSTONE]);
        AddRecipe(new ItemStack(Item.REPEATER, 1), ["#X#", "III", '#', Block.LIT_REDSTONE_TORCH, 'X', Item.REDSTONE, 'I', Block.STONE]);
        AddRecipe(new ItemStack(Item.CLOCK, 1), [" # ", "#X#", " # ", '#', Item.GOLD_INGOT, 'X', Item.REDSTONE]);
        AddRecipe(new ItemStack(Item.COMPASS, 1), [" # ", "#X#", " # ", '#', Item.IRON_INGOT, 'X', Item.REDSTONE]);
        AddRecipe(new ItemStack(Item.MAP, 1), ["###", "#X#", "###", '#', Item.PAPER, 'X', Item.COMPASS]);
        AddRecipe(new ItemStack(Block.BUTTON, 1), ["#", "#", '#', Block.STONE]);
        AddRecipe(new ItemStack(Block.STONE_PRESSURE_PLATE, 1), ["##", '#', Block.STONE]);
        AddRecipe(new ItemStack(Block.WOODEN_PRESSURE_PLATE, 1), ["##", '#', Block.PLANKS]);
        AddRecipe(new ItemStack(Block.DISPENSER, 1), ["###", "#X#", "#R#", '#', Block.COBBLESTONE, 'X', Item.BOW, 'R', Item.REDSTONE]);
        AddRecipe(new ItemStack(Block.PISTON, 1), ["TTT", "#X#", "#R#", '#', Block.COBBLESTONE, 'X', Item.IRON_INGOT, 'R', Item.REDSTONE, 'T', Block.PLANKS]);
        AddRecipe(new ItemStack(Block.STICKY_PISTON, 1), ["S", "P", 'S', Item.SLIMEBALL, 'P', Block.PISTON]);
        AddRecipe(new ItemStack(Item.BED, 1), ["###", "XXX", '#', Block.WOOL, 'X', Block.PLANKS]);

        _recipes.Sort(new RecipeSorter());

        Console.WriteLine($"{_recipes.Count} recipes");
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