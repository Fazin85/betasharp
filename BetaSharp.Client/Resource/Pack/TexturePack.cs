namespace BetaSharp.Client.Resource.Pack;

public abstract class TexturePack
{
    public string? TexturePackFileName;
    public string? FirstDescriptionLine;
    public string? SecondDescriptionLine;
    public string? Signature;

    public virtual void func_6482_a()
    {
    }

    public virtual void CloseTexturePackFile()
    {
    }

    public virtual void func_6485_a(Minecraft mc)
    {
    }

    public virtual void Unload(Minecraft mc)
    {
    }

    public virtual void BindThumbnailTexture(Minecraft mc)
    {
    }

    public virtual Stream? GetResourceAsStream(string path)
    {
        var asset = AssetManager.Instance.getAsset(path);
        if (asset == null) return null;
        return new MemoryStream(asset.getBinaryContent());
    }
}