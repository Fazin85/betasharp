namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public readonly ref struct FaceColors
{
    public readonly float RedTopLeft, GreenTopLeft, BlueTopLeft;
    public readonly float RedBottomLeft, GreenBottomLeft, BlueBottomLeft;
    public readonly float RedBottomRight, GreenBottomRight, BlueBottomRight;
    public readonly float RedTopRight, GreenTopRight, BlueTopRight;

    public FaceColors(
        float rTL, float gTL, float bTL,
        float rBL, float gBL, float bBL,
        float rBR, float gBR, float bBR,
        float rTR, float gTR, float bTR)
    {
        RedTopLeft = rTL; GreenTopLeft = gTL; BlueTopLeft = bTL;
        RedBottomLeft = rBL; GreenBottomLeft = gBL; BlueBottomLeft = bBL;
        RedBottomRight = rBR; GreenBottomRight = gBR; BlueBottomRight = bBR;
        RedTopRight = rTR; GreenTopRight = gTR; BlueTopRight = bTR;
    }
}
