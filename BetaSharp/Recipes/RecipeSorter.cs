namespace BetaSharp.Recipes;

public class RecipeSorter : IComparer<IRecipe>
{
    public int Compare(IRecipe x, IRecipe y)
    {
        if (x == null || y == null) return 0;

        // 1. Check types (Shapeless vs Shaped)
        // Using C# pattern matching (is)
        if (x is ShapelessRecipes && y is ShapedRecipes) return 1;
        if (y is ShapelessRecipes && x is ShapedRecipes) return -1;

        // 2. Compare Sizes
        int xSize = x.GetRecipeSize();
        int ySize = y.GetRecipeSize();

        // Standard C# comparison (returns -1, 0, or 1)
        return ySize.CompareTo(xSize); 
    }
}