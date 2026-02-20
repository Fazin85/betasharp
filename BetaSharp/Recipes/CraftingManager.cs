using BetaSharp.Blocks;
using BetaSharp.Inventorys;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class CraftingManager
{
    private static CraftingManager instance { get; } = new();
    public List<IRecipe> Recipes { get; } = [];

    public static CraftingManager getInstance()
    {
        return instance;
    }

   private CraftingManager()
    {
        RecipeLoader.LoadAll(this, "Assets/Recipes/recipes.json");
        Recipes.Sort(new RecipeSorter());
        Log.Info($"[CraftingManager] {Recipes.Count} recipes loaded from data.");
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

        Recipes.Add(new ShapedRecipes(width, height, ingredientGrid, result));
    }

    public void AddShapelessRecipe(ItemStack result, params object[] pattern)
    {
        List<ItemStack> stacks = [];

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

        Recipes.Add(new ShapelessRecipes(result, stacks));
    }

    public ItemStack? FindMatchingRecipe(InventoryCrafting craftingInventory)
    {
        return Recipes
            .FirstOrDefault(r => r.Matches(craftingInventory))
            ?.GetCraftingResult(craftingInventory);
    }
}
