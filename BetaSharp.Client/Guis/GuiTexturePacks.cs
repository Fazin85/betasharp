using System.Diagnostics;
using BetaSharp.Client.Rendering;

namespace BetaSharp.Client.Guis;

public class GuiTexturePacks : GuiScreen
{
    private const int BUTTON_OPEN_FOLDER = 5;
    private const int BUTTON_DONE = 6;

    protected GuiScreen parentScreen;
    private int refreshTimer = -1;
    private string texturePackFolder = "";
    private GuiTexturePackSlot guiTexturePackSlot;

    public GuiTexturePacks(GuiScreen parent)
    {
        parentScreen = parent;
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        controlList.add(new GuiSmallButton(BUTTON_OPEN_FOLDER, width / 2 - 154, height - 48, translations.translateKey("texturePack.openFolder")));
        controlList.add(new GuiSmallButton(BUTTON_DONE, width / 2 + 4, height - 48, translations.translateKey("gui.done")));
        mc.texturePackList.updateAvaliableTexturePacks();
        texturePackFolder = new java.io.File(Minecraft.getMinecraftDir(), "texturepacks").getAbsolutePath();
        guiTexturePackSlot = new GuiTexturePackSlot(this);
        guiTexturePackSlot.registerScrollButtons(controlList, 7, 8);
    }

    protected override void actionPerformed(GuiButton var1)
    {
        if (var1.enabled)
        {
            switch (var1.id)
            {
                case BUTTON_OPEN_FOLDER:
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "file://" + texturePackFolder,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to open URL: {ex.Message}");
                    }
                    break;
                case BUTTON_DONE:
                    mc.textureManager.reload();
                    mc.displayGuiScreen(parentScreen);
                    break;
                default:
                    guiTexturePackSlot.actionPerformed(var1);
                    break;
            }

        }
    }

    protected override void mouseClicked(int mouseX, int mouseY, int button)
    {
        base.mouseClicked(mouseX, mouseY, button);
    }

    protected override void mouseMovedOrUp(int mouseX, int mouseY, int button)
    {
        base.mouseMovedOrUp(mouseX, mouseY, button);
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        guiTexturePackSlot.drawScreen(mouseX, mouseY, partialTicks);
        if (refreshTimer <= 0)
        {
            mc.texturePackList.updateAvaliableTexturePacks();
            refreshTimer += 20;
        }

        TranslationStorage translations = TranslationStorage.getInstance();
        drawCenteredString(fontRenderer, translations.translateKey("texturePack.title"), width / 2, 16, 0x00FFFFFF);
        drawCenteredString(fontRenderer, translations.translateKey("texturePack.folderInfo"), width / 2 - 77, height - 26, 8421504);
        base.render(mouseX, mouseY, partialTicks);
    }

    public override void updateScreen()
    {
        base.updateScreen();
        --refreshTimer;
    }

    public static Minecraft func_22124_a(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22126_b(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22119_c(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22122_d(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22117_e(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22118_f(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22116_g(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22121_h(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static Minecraft func_22123_i(GuiTexturePacks var0)
    {
        return var0.mc;
    }

    public static TextRenderer func_22127_j(GuiTexturePacks var0)
    {
        return var0.fontRenderer;
    }

    public static TextRenderer func_22120_k(GuiTexturePacks var0)
    {
        return var0.fontRenderer;
    }

    public static TextRenderer func_22125_l(GuiTexturePacks var0)
    {
        return var0.fontRenderer;
    }
}
