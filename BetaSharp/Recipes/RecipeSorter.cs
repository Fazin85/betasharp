using java.util;
using java.util.function;

namespace BetaSharp.Recipes;

public class RecipeSorter : Comparator
{
    private static int compareRecipes(IRecipe a, IRecipe b)
    {
        return a is ShapelessRecipes && b is ShapedRecipes ? 1 : (b is ShapelessRecipes && a is ShapedRecipes ? -1 : (b.GetRecipeSize() < a.GetRecipeSize() ? -1 : (b.GetRecipeSize() > a.GetRecipeSize() ? 1 : 0)));
    }

    public int compare(object a, object b)
    {
        return compareRecipes((IRecipe)a, (IRecipe)b);
    }

    public Comparator thenComparing(Comparator other)
    {
        throw new NotImplementedException();
    }

    public bool equals(object other)
    {
        throw new NotImplementedException();
    }

    public Comparator reversed()
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparing(Function keyExtractor, Comparator keyComparator)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparing(Function keyExtractor)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparingInt(ToIntFunction keyExtractor)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparingLong(ToLongFunction keyExtractor)
    {
        throw new NotImplementedException();
    }

    public Comparator thenComparingDouble(ToDoubleFunction keyExtractor)
    {
        throw new NotImplementedException();
    }
}