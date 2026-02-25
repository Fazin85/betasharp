namespace BetaSharp.Util.Maths;

public class Facings
{
    public static readonly int[] TO_DIR = [3, 4, 2, 5];
    public static readonly int[] OPPOSITE = [2, 3, 0, 1];
    public static readonly int[][] BED_FACINGS = [[1, 0, 3, 2, 5, 4], [1, 0, 5, 4, 2, 3], [1, 0, 2, 3, 4, 5], [1, 0, 4, 5, 3, 2]];
}
