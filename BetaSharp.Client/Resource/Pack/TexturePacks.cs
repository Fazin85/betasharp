namespace BetaSharp.Client.Resource.Pack;

public class TexturePacks
{
    private List<TexturePack> _availTexturePacks = [];
    private readonly TexturePack _defaultTexturePack = new BuiltInTexturePack();
    public TexturePack SelectedTexturePack;
    private readonly Dictionary<string, TexturePack> _texturePacks = [];
    private readonly Minecraft _mc;
    private readonly DirectoryInfo _texturePackDir;
    private string? _currentTexturePack;
    public List<TexturePack> AvailableTexturePacks => _availTexturePacks;
    
    public TexturePacks(Minecraft mc, DirectoryInfo texturePackDir)
    {
        _mc = mc;
        _texturePackDir = new DirectoryInfo(System.IO.Path.Combine(texturePackDir.FullName, "texturepacks"));
        if (!_texturePackDir.Exists)
        {
            _texturePackDir.Create();
        }

        _currentTexturePack = mc.options.skin;
        updateAvaliableTexturePacks();
        SelectedTexturePack.func_6482_a();
    }

    public bool setTexturePack(TexturePack texturePack)
    {
        if (texturePack == SelectedTexturePack)
        {
            return false;
        }

        SelectedTexturePack.CloseTexturePackFile();
        _currentTexturePack = texturePack.TexturePackFileName;
        SelectedTexturePack = texturePack;

        _mc.options.skin = _currentTexturePack;
        _mc.options.saveOptions();

        SelectedTexturePack.func_6482_a();
        return true;

    }

    public void updateAvaliableTexturePacks()
    {
        List<TexturePack> availablePacks = [];
        SelectedTexturePack = null!;
        availablePacks.Add(_defaultTexturePack);

        if (_texturePackDir.Exists)
        {
            foreach (FileInfo file in _texturePackDir.GetFiles("*.zip"))
            {
                string signature = $"{file.Name}:{file.Length}:{file.LastWriteTimeUtc.Ticks}";

                try
                {
                    if (!_texturePacks.TryGetValue(signature, out TexturePack? cachedPack))
                    {
                        ZippedTexturePack newPack = new(file)
                        {
                            Signature = signature
                        };
                        _texturePacks[signature] = newPack;
                        newPack.func_6485_a(_mc);
                        cachedPack = newPack;
                    }

                    if (cachedPack.TexturePackFileName == _currentTexturePack)
                    {
                        SelectedTexturePack = cachedPack;
                    }

                    availablePacks.Add(cachedPack);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

            }
        }

        SelectedTexturePack ??= _defaultTexturePack;

        foreach (TexturePack oldPack in _availTexturePacks)
        {
            if (!availablePacks.Contains(oldPack))
            {
                oldPack.Unload(_mc);
                if (oldPack.Signature != null)
                {
                    _texturePacks.Remove(oldPack.Signature);
                }
            }
        }

        _availTexturePacks = availablePacks;
    }
}