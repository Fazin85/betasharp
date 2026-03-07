using System.Collections.Generic;

namespace BetaSharp.Recipes.Data;

public class RecipeModel
{
    public string Type { get; set; } = "";
    public CraftingRecipeModel Result { get; set; } = new();
    public string[]? Pattern { get; set; }
    public Dictionary<string, string>? Key { get; set; } 
    public string[]? Ingredients { get; set; }
}

public class CraftingRecipeModel
{
    public string Name { get; set; } = "";
    public int Count { get; set; } = 1;
    public int Meta { get; set; } = -1;
}

public class SmeltingRecipeModel
{
    public string Input { get; set; } = "";
    public CraftingRecipeModel Result { get; set; } = new();
}