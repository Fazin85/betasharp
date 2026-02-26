using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks;

public readonly ref struct BlockRenderContext
{
    public readonly int OverrideTexture;
    public readonly bool RenderAllFaces;
    public readonly int AoBlendMode;
    public readonly Box? OverrideBounds;

    public BlockRenderContext(int overrideTexture = -1, bool renderAllFaces = false, int aoMode = 1, Box? bounds = null)
    {
        OverrideTexture = overrideTexture;
        RenderAllFaces = renderAllFaces;
        AoBlendMode = aoMode;
        OverrideBounds = bounds;
    }
}
