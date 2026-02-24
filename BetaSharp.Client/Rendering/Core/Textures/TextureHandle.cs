namespace BetaSharp.Client.Rendering.Core.Textures;

public class TextureHandle
{
    private readonly TextureManager _manager;
    public GLTexture? Texture { get; internal set; }
    public int Id => (int)(Texture?.Id ?? 0u);

    internal TextureHandle(TextureManager manager, GLTexture? texture)
    {
        _manager = manager;
        Texture = texture;
    }

    public void Bind()
    {
        Texture?.Bind();
    }

    public override string ToString()
    {
        return $"TextureHandle(Id={Id}, Source={Texture?.Source ?? "null"})";
    }
}
