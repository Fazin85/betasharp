using System.Text.Json;
using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Recipes.Data;

namespace BetaSharp.Recipes;

public static class RecipeLoader
{
    public static void LoadAll(CraftingManager manager, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Log.Info($"[RecipeLoader] WARNING: Could not find recipes file at {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        var recipes = JsonSerializer.Deserialize<List<RecipeModel>>(json);

        if (recipes == null) return;

        foreach (var recipe in recipes)
        {
            ItemStack resultStack = BetaSharp.Registry.ItemRegistry.ResolveStack(recipe.Result.Name, recipe.Result.Count, recipe.Result.Meta);

            if (recipe.Type == "shaped")
            {
                ParseShaped(manager, recipe, resultStack);
            }
            else if (recipe.Type == "shapeless")
            {
                ParseShapeless(manager, recipe, resultStack);
            }
        }

        Log.Info($"[RecipeLoader] Successfully loaded {recipes.Count} recipes from JSON!");
    }

    private static void ParseShaped(CraftingManager manager, RecipeModel recipe, ItemStack result)
    {
        var parameters = new List<object>();

        if (recipe.Pattern != null)
        {
            foreach (var row in recipe.Pattern) parameters.Add(row);
        }

        if (recipe.Key != null)
        {
            foreach (var kvp in recipe.Key)
            {
                parameters.Add(kvp.Key[0]);
                parameters.Add(BetaSharp.Registry.ItemRegistry.Resolve(kvp.Value));
            }
        }

        manager.AddRecipe(result, parameters.ToArray());
    }

    private static void ParseShapeless(CraftingManager manager, RecipeModel recipe, ItemStack result)
    {
        var parameters = new List<object>();

        if (recipe.Ingredients != null)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                parameters.Add(BetaSharp.Registry.ItemRegistry.Resolve(ingredient));
            }
        }

        manager.AddShapelessRecipe(result, parameters.ToArray());
    }

    public static void LoadSmelting(SmeltingRecipeManager manager, string filePath)
    {
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        var recipes = JsonSerializer.Deserialize<List<SmeltingRecipeModel>>(json);

        if (recipes == null) return;

        foreach (var recipe in recipes)
        {
            object inputObj = BetaSharp.Registry.ItemRegistry.Resolve(recipe.Input);
            int inputId = inputObj switch {
                Item i => i.id,
                Block b => b.id,
                _ => 0
            };

            ItemStack output = BetaSharp.Registry.ItemRegistry.ResolveStack(
                recipe.Result.Name, 
                recipe.Result.Count, 
                recipe.Result.Meta
            );

            manager.AddSmelting(inputId, output);
        }
    }
}