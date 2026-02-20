using System.Diagnostics;
using BetaSharp.Client.Rendering;

namespace BetaSharp.Client.Guis;

public class GuiTexturePacks : GuiScreen
{
    private const int ButtonOpenFolder = 5;
    private const int ButtonDone = 6;

    protected GuiScreen _parentScreen;
    private int _refreshTimer = -1;
    private string _texturePackFolder = "";
    private GuiTexturePackSlot _guiTexturePackSlot;

    public GuiTexturePacks(GuiScreen parent)
    {
        _parentScreen = parent;
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        _controlList.Add(new GuiSmallButton(ButtonOpenFolder, Width / 2 - 154, Height - 48, translations.translateKey("texturePack.openFolder")));
        _controlList.Add(new GuiSmallButton(ButtonDone, Width / 2 + 4, Height - 48, translations.translateKey("gui.done")));
        mc.texturePackList.updateAvaliableTexturePacks();
        _texturePackFolder = new java.io.File(Minecraft.getMinecraftDir(), "texturepacks").getAbsolutePath();
        _guiTexturePackSlot = new GuiTexturePackSlot(this);
        _guiTexturePackSlot.RegisterScrollButtons(_controlList, 7, 8);
    }

    protected override void ActionPerformed(GuiButton btn)
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
                        Log.Error($"Failed to open URL: {ex.Message}");
                    }
                    break;
                case ButtonDone:
                    mc.textureManager.Reload();
                    mc.displayGuiScreen(_parentScreen);
                    break;
                default:
                    _guiTexturePackSlot.actionPerformed(btn);
                    break;
            }

        }
    }

    protected override void MouseClicked(int mouseX, int mouseY, int button)
    {
        base.MouseClicked(mouseX, mouseY, button);
    }

    protected override void MouseMovedOrUp(int mouseX, int mouseY, int button)
    {
        base.MouseMovedOrUp(mouseX, mouseY, button);
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        _guiTexturePackSlot.drawScreen(mouseX, mouseY, partialTicks);
        if (_refreshTimer <= 0)
        {
            mc.texturePackList.updateAvaliableTexturePacks();
            _refreshTimer += 20;
        }

        TranslationStorage translations = TranslationStorage.getInstance();
        DrawCenteredString(FontRenderer, translations.translateKey("texturePack.title"), Width / 2, 16, 0xFFFFFF);
        DrawCenteredString(FontRenderer, translations.translateKey("texturePack.folderInfo"), Width / 2 - 77, Height - 26, 0x808080);
        base.Render(mouseX, mouseY, partialTicks);
    }

    public override void UpdateScreen()
    {
        base.UpdateScreen();
        --_refreshTimer;
    }
}
