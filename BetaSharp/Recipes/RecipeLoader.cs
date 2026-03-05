using System.Text.Json;
using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Recipes.Data;
using BetaSharp.Registry;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Recipes;

public static class RecipeLoader
{
    private static readonly ILogger s_logger = Log.Instance.For(nameof(RecipeLoader));

    public sealed record RecipeValidationError(string Source, string Id, string Message);

    public static void LoadAll(CraftingManager manager, string filePath)
    {
        if (!File.Exists(filePath))
        {
            s_logger.LogWarning($"WARNING: Could not find recipes file at {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        var recipes = JsonSerializer.Deserialize<List<RecipeModel>>(json);

        if (recipes == null) return;

        foreach (var recipe in recipes)
        {
            ItemStack resultStack = ItemRegistry.ResolveStack(recipe.Result.Name, recipe.Result.Count, recipe.Result.Meta);

            if (recipe.Type == "shaped")
            {
                ParseShaped(manager, recipe, resultStack);
            }
            else if (recipe.Type == "shapeless")
            {
                ParseShapeless(manager, recipe, resultStack);
            }
        }

        s_logger.LogWarning($"Successfully loaded {recipes.Count}!");
    }

    public static List<RecipeValidationError> ValidateRecipes(string filePath)
    {
        var errors = new List<RecipeValidationError>();

        if (!File.Exists(filePath))
        {
            return errors;
        }

        string json = File.ReadAllText(filePath);
        var recipes = JsonSerializer.Deserialize<List<RecipeModel>>(json);
        if (recipes == null) return errors;

        foreach (var recipe in recipes)
        {
            // Validate result
            try
            {
                _ = ResourceLocation.Parse(recipe.Result.Name);
                _ = ItemRegistry.ResolveStack(recipe.Result.Name, recipe.Result.Count, recipe.Result.Meta);
            }
            catch (Exception ex)
            {
                errors.Add(new RecipeValidationError("recipe-result", recipe.Result.Name, ex.Message));
            }

            // Validate shaped/shapeless ingredients
            if (recipe.Type == "shaped" && recipe.Key != null)
            {
                foreach (var kvp in recipe.Key)
                {
                    try
                    {
                        _ = ResourceLocation.Parse(kvp.Value);
                        _ = ItemRegistry.Resolve(kvp.Value);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new RecipeValidationError("recipe-key", kvp.Value, ex.Message));
                    }
                }
            }
            else if (recipe.Type == "shapeless" && recipe.Ingredients != null)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    try
                    {
                        _ = ResourceLocation.Parse(ingredient);
                        _ = ItemRegistry.Resolve(ingredient);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new RecipeValidationError("recipe-ingredient", ingredient, ex.Message));
                    }
                }
            }
        }

        return errors;
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
                parameters.Add(Registry.ItemRegistry.Resolve(kvp.Value));
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
                parameters.Add(Registry.ItemRegistry.Resolve(ingredient));
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
            object inputObj = Registry.ItemRegistry.Resolve(recipe.Input);
            int inputId = inputObj switch {
                Item i => i.id,
                Block b => b.id,
                _ => 0
            };

            ItemStack output = Registry.ItemRegistry.ResolveStack(
                recipe.Result.Name,
                recipe.Result.Count,
                recipe.Result.Meta
            );

            manager.AddSmelting(inputId, output);
        }
    }

    public static List<RecipeValidationError> ValidateSmelting(string filePath)
    {
        var errors = new List<RecipeValidationError>();

        if (!File.Exists(filePath))
        {
            return errors;
        }

        string json = File.ReadAllText(filePath);
        var recipes = JsonSerializer.Deserialize<List<SmeltingRecipeModel>>(json);
        if (recipes == null) return errors;

        foreach (var recipe in recipes)
        {
            try
            {
                _ = ResourceLocation.Parse(recipe.Input);
                _ = ItemRegistry.Resolve(recipe.Input);
            }
            catch (Exception ex)
            {
                errors.Add(new RecipeValidationError("smelting-input", recipe.Input, ex.Message));
            }

            try
            {
                _ = ResourceLocation.Parse(recipe.Result.Name);
                _ = ItemRegistry.ResolveStack(
                    recipe.Result.Name,
                    recipe.Result.Count,
                    recipe.Result.Meta
                );
            }
            catch (Exception ex)
            {
                errors.Add(new RecipeValidationError("smelting-output", recipe.Result.Name, ex.Message));
            }
        }

        return errors;
    }
}
