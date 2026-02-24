namespace BetaSharp.Client.Guis;

public struct Size
{
    public int Width;
    public int Height;

    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public static implicit operator Size((int, int) tuple) => new(tuple.Item1, tuple.Item2);
}
