namespace BetaSharp.Recipes;

public class RecipeSorter : IComparer<IRecipe>
{
    public int Compare(IRecipe x, IRecipe y)
    {
        if (x == null || y == null) return 0;

        if (x is ShapelessRecipes && y is ShapedRecipes) return 1;
        if (y is ShapelessRecipes && x is ShapedRecipes) return -1;

        int xSize = x.GetRecipeSize();
        int ySize = y.GetRecipeSize();

        return ySize.CompareTo(xSize); 
    }
}