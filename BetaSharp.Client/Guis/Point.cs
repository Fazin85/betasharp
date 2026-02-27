namespace BetaSharp.Client.Guis;

public struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator Point((int, int) tuple) => new(tuple.Item1, tuple.Item2);
}
