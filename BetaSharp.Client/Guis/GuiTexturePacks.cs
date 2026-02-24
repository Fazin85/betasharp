using System.Diagnostics;
using BetaSharp.Client.Rendering;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Guis;

public class GuiTexturePacks : GuiScreen
{
    private readonly ILogger<GuiTexturePacks> _logger = Log.Instance.For<GuiTexturePacks>();

    private const int ButtonOpenFolder = 5;
    private const int ButtonDone = 6;

    protected GuiScreen _parentScreen;
    private int _refreshTimer = -1;
    private string _texturePackFolder = "";
    private GuiTexturePackList _guiTexturePackList;

    public GuiTexturePacks(GuiScreen parent)
    {
        _parentScreen = parent;
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.Instance;
        Children.Add(new GuiSmallButton(ButtonOpenFolder, Width / 2 - 154, Height - 48, translations.TranslateKey("texturePack.openFolder")));
        Children.Add(new GuiSmallButton(ButtonDone, Width / 2 + 4, Height - 48, translations.TranslateKey("gui.done")));
        mc.texturePackList.updateAvaliableTexturePacks();
        _texturePackFolder = new java.io.File(Minecraft.getMinecraftDir(), "texturepacks").getAbsolutePath();
        _guiTexturePackList = new GuiTexturePackList(this);
        _guiTexturePackList.RegisterScrollButtons(Children, 7, 8);
    }

    protected override void ActionPerformed(Button btn)
    {
        if (btn.Enabled)
        {
            switch (btn.Id)
            {
                case ButtonOpenFolder:
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "file://" + _texturePackFolder,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to open URL: {ex.Message}");
                    }
                    break;
                case ButtonDone:
                    mc.textureManager.Reload();
                    mc.OpenScreen(_parentScreen);
                    break;
                default:
                    _guiTexturePackList.ActionPerformed(btn);
                    break;
            }

        }
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        _guiTexturePackList.DrawScreen(mouseX, mouseY, tickDelta);
        if (_refreshTimer <= 0)
        {
            mc.texturePackList.updateAvaliableTexturePacks();
            _refreshTimer += 20;
        }

        TranslationStorage translations = TranslationStorage.Instance;
        Gui.DrawCenteredString(FontRenderer, translations.TranslateKey("texturePack.title"), Width / 2, 16, 0xFFFFFF);
        Gui.DrawCenteredString(FontRenderer, translations.TranslateKey("texturePack.folderInfo"), Width / 2 - 77, Height - 26, 0x808080);
        base.OnRendered(mouseX, mouseY, tickDelta);
    }

    public override void UpdateScreen()
    {
        --_refreshTimer;
    }
}
