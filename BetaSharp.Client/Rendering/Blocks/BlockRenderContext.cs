using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks;

public readonly ref struct BlockRenderContext
{
    public readonly int OverrideTexture;
    public readonly bool RenderAllFaces;
    public readonly bool FlipTexture;
    public readonly Box? OverrideBounds;
    public readonly bool EnableAo = true;
    public readonly int AoBlendMode = 0;

    // UV Rotations
    public readonly int UvRotateTop;
    public readonly int UvRotateBottom;
    public readonly int UvRotateNorth;
    public readonly int UvRotateSouth;
    public readonly int UvRotateEast;
    public readonly int UvRotateWest;

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

        CustomFlag = customFlag;
        AoBlendMode = aoBlendMode;
    }
}
