using BetaSharp;
using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiLanguage : GuiScreen
{
    private const int ButtonScrollUp = 4;
    private const int ButtonScrollDown = 5;
    private const int ButtonBack = 1000;

    private readonly GuiScreen _parentScreen;
    private GuiLanguageSlot _languageSlot;

    public GuiLanguage(GuiScreen parentScreen)
    {
        _parentScreen = parentScreen;
    }

    public override void initGui()
    {
        _languageSlot = new GuiLanguageSlot(this);
        _languageSlot.registerScrollButtons(controlList, ButtonScrollUp, ButtonScrollDown);
        //controlList.add(new GuiButton(ButtonScrollUp, width / 2 - 154, height - 52, 44, 20, "^"));
        //controlList.add(new GuiButton(ButtonScrollDown, width / 2 - 154, height - 28, 44, 20, "v"));
        controlList.add(new GuiButton(ButtonBack, width / 2 - 100, height - 28, 200, 20, TranslationStorage.Instance.TranslateKey("gui.done")));
    }

    public void SelectLanguageAndBack(int index)
    {
        var locales = TranslationStorage.GetAvailableLocales();
        if (index >= 0 && index < locales.Count)
        {
            TranslationStorage.Instance.SetLanguage(locales[index].Code);
            mc.displayGuiScreen(_parentScreen);
        }
    }

    protected override void actionPerformed(GuiButton button)
    {
        if (button.id == ButtonBack)
        {
            mc.displayGuiScreen(_parentScreen);
            return;
        }
        if (button.id == ButtonScrollUp || button.id == ButtonScrollDown)
        {
            _languageSlot.actionPerformed(button);
            return;
        }
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        _languageSlot.drawScreen(mouseX, mouseY, partialTicks);
        string title = TranslationStorage.Instance.TranslateKey("menu.language");
        drawCenteredString(fontRenderer, title, width / 2, 12, 0x00FFFFFF);
        base.render(mouseX, mouseY, partialTicks);
    }
}
