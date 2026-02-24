using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Textures;

public class DynamicTexture : java.lang.Object
{
    public byte[] pixels = new byte[1024];
    public int sprite;
    public bool anaglyphEnabled = false;
    public int copyTo = 0;
    public int replicate = 1;
    public FXImage atlas = FXImage.Terrain;

    public enum FXImage
    {
        Terrain,
        Items
    }

    public DynamicTexture(int iconIdx)
    {
        sprite = iconIdx;
    }

    public virtual void tick()
    {
    }
}
