namespace BetaSharp;

public class ScaledResolution
{
    public int ScaledWidth { get; private set; }
    public int ScaledHeight { get; private set; }
    public double ScaledWidthDouble { get; private set; }
    public double ScaledHeightDouble { get; private set; }
    private readonly int scaleFactor;

    public ScaledResolution(GameOptions options, int scaledWidth, int scaledHeight)
    {
        ScaledWidth = scaledWidth;
        ScaledHeight = scaledHeight;
        int guiScale = options.guiScale;
        scaleFactor = 1;
            
        if (guiScale == 0)
            guiScale = 1000;

        while (scaleFactor < guiScale && ScaledWidth / (scaleFactor + 1) >= 320 && ScaledHeight / (scaleFactor + 1) >= 240)
        {
            ++scaleFactor;
        }

        ScaledWidthDouble = ScaledWidth / (double)scaleFactor;
        ScaledHeightDouble = ScaledHeight / (double)scaleFactor;
        ScaledWidth = (int)Math.Ceiling(ScaledWidthDouble);
        ScaledHeight = (int)Math.Ceiling(ScaledHeightDouble);
    }
}