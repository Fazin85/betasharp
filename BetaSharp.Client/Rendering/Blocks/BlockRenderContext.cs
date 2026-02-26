using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks;

public ref struct BlockRenderContext
{
    public int OverrideTexture;
    public bool RenderAllFaces;
    public bool FlipTexture;
    public Box? OverrideBounds;
    public bool EnableAo = true;
    public int AoBlendMode = 0;

    // UV Rotations
    public int UvRotateTop;
    public int UvRotateBottom;
    public int UvRotateNorth;
    public int UvRotateSouth;
    public int UvRotateEast;
    public int UvRotateWest;

    // Custom flag for Pistons (Expanded/Short arm)
    public readonly bool CustomFlag;

    public BlockRenderContext(
        int overrideTexture = -1,
        bool renderAllFaces = false,
        bool flipTexture = false,
        Box? bounds = null,
        int uvTop = 0, int uvBottom = 0,
        int uvNorth = 0, int uvSouth = 0,
        int uvEast = 0, int uvWest = 0,
        bool customFlag = false,
        bool enableAo = true,
        int aoBlendMode = 0)
    {
        OverrideTexture = overrideTexture;
        RenderAllFaces = renderAllFaces;
        FlipTexture = flipTexture;
        OverrideBounds = bounds;

        UvRotateTop = uvTop;
        UvRotateBottom = uvBottom;
        UvRotateNorth = uvNorth;
        UvRotateSouth = uvSouth;
        UvRotateEast = uvEast;
        UvRotateWest = uvWest;

        AoBlendMode = aoBlendMode;
        EnableAo = enableAo;

        CustomFlag = customFlag;
    }
}
