using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiOptions : GuiScreen
{
    private const int ButtonVideoSettings = 101;
    private const int ButtonAudioSettings = 102;
    private const int ButtonControls = 100;
    private const int ButtonDone = 200;

    private readonly GuiScreen _parentScreen;
    private readonly GameOptions _options;

    protected string _screenTitle = "Options";

    public GuiOptions(GuiScreen parentScreen, GameOptions gameOptions)
    {
        _parentScreen = parentScreen;
        _options = gameOptions;
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.Instance;
        _screenTitle = translations.TranslateKey("options.title");
        int rowIndex = 0;

        foreach (GameOption option in _options.MainScreenOptions)
        {
            int xPos = Width / 2 - 155 + (rowIndex % 2 * 160);
            int yPos = Height / 6 + 24 * (rowIndex >> 1);
            int id = rowIndex;

            if (option is FloatOption floatOpt)
            {
                _controlList.Add(new GuiSlider(id, xPos, yPos, floatOpt, option.GetDisplayString(translations), floatOpt.Value));
            }
            else
            {
                _controlList.Add(new GuiSmallButton(id, xPos, yPos, option, option.GetDisplayString(translations)));
            }

            ++rowIndex;
        }

        _controlList.Add(new GuiButton(ButtonVideoSettings, Width / 2 - 100, Height / 6 + 72 + 12, translations.TranslateKey("options.video")));
        _controlList.Add(new GuiButton(ButtonAudioSettings, Width / 2 - 100, Height / 6 + 96 + 12, "Audio Settings"));
        _controlList.Add(new GuiButton(ButtonControls, Width / 2 - 100, Height / 6 + 120 + 12, translations.TranslateKey("options.controls")));
        _controlList.Add(new GuiButton(ButtonDone, Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done")));
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (!button.Enabled) return;

        if (button is GuiSmallButton smallBtn && smallBtn.Option != null)
        {
            smallBtn.ClickOption();
            button.DisplayString = smallBtn.Option.GetDisplayString(TranslationStorage.Instance);
        }

        switch (button.Id)
        {
            case ButtonVideoSettings:
                mc.options.SaveOptions();
                mc.displayGuiScreen(new GuiVideoSettings(this, _options));
                break;
            case ButtonAudioSettings:
                mc.options.SaveOptions();
                mc.displayGuiScreen(new GuiAudio(this, _options));
                break;
            case ButtonControls:
                mc.options.SaveOptions();
                mc.displayGuiScreen(new GuiControls(this, _options));
                break;
            case ButtonDone:
                mc.options.SaveOptions();
                mc.displayGuiScreen(_parentScreen);
                break;
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(FontRenderer, _screenTitle, Width / 2, 20, 0xFFFFFF);

        base.Render(mouseX, mouseY, partialTicks);
    }
}
