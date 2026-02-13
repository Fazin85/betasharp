using java.util;
using java.util.function;

namespace BetaSharp.Recipes;

public class RecipeSorter : Comparator
{
    private static int compareRecipes(IRecipe var1, IRecipe var2)
    {
        return var1 is ShapelessRecipes && var2 is ShapedRecipes ? 1 : (var2 is ShapelessRecipes && var1 is ShapedRecipes ? -1 : (var2.getRecipeSize() < var1.getRecipeSize() ? -1 : (var2.getRecipeSize() > var1.getRecipeSize() ? 1 : 0)));
    }

    public int compare(object var1, object var2)
    {
        return compareRecipes((IRecipe)var1, (IRecipe)var2);
    }

    public Comparator thenComparing(Comparator other)
    {
        throw new NotImplementedException();
    }

    public bool equals(object obj)
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