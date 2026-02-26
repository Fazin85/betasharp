namespace BetaSharp.Client.Rendering.Blocks;

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

    public static FaceColors AssignVertexColors(float v0, float v1, float v2, float v3, float r, float g, float b, float faceShadow, bool tint)
    {
        float tr = (tint ? r : 1.0F) * faceShadow;
        float tg = (tint ? g : 1.0F) * faceShadow;
        float tb = (tint ? b : 1.0F) * faceShadow;

        return new FaceColors(
            tr * v0, tg * v0, tb * v0, // Top Left
            tr * v1, tg * v1, tb * v1, // Bottom Left
            tr * v2, tg * v2, tb * v2, // Bottom Right
            tr * v3, tg * v3, tb * v3  // Top Right
        );
    }
}
