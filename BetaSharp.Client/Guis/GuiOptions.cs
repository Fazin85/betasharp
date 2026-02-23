using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiOptions : GuiScreen
{
    private const int ButtonVideoSettings = 101;
    private const int ButtonControls = 100;
    private const int ButtonDone = 200;

    private readonly GuiScreen _parentScreen;
    private readonly GameOptions _options;

    protected string _screenTitle = "Options";
    private static readonly EnumOptions[] _availableOptions = [
        EnumOptions.Music,
        EnumOptions.Sound,
        EnumOptions.Difficulty,
        EnumOptions.Fov
    ];

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

        foreach (EnumOptions currentOption in _availableOptions)
        {
            int xPos = Width / 2 - 155 + (rowIndex % 2 * 160);
            int yPos = Height / 6 + 24 * (rowIndex >> 1);
            if (!currentOption.IsFloat())
            {
                _controlList.Add(new GuiSmallButton((int)currentOption, xPos, yPos, currentOption, _options.GetKeyBinding(currentOption)));
            }
            else
            {
                _controlList.Add(new GuiSlider((int)currentOption, xPos, yPos, currentOption, _options.GetKeyBinding(currentOption), _options.GetOptionFloatValue(currentOption)));
            }

            ++rowIndex;
        }

        _controlList.Add(new GuiButton(ButtonVideoSettings, Width / 2 - 100, Height / 6 + 96 + 12, translations.TranslateKey("options.video")));
        _controlList.Add(new GuiButton(ButtonControls, Width / 2 - 100, Height / 6 + 120 + 12, translations.TranslateKey("options.controls")));
        _controlList.Add(new GuiButton(ButtonDone, Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done")));
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (!button.Enabled) return;

        if (button.Id < 100 && button is GuiSmallButton)
        {
            _options.SetOptionValue(((GuiSmallButton)button).returnEnumOptions(), 1);
            button.DisplayString = _options.GetKeyBinding((EnumOptions)button.Id);
        }

        switch (button.Id)
        {
            case ButtonVideoSettings:
                mc.options.SaveOptions();
                mc.displayGuiScreen(new GuiVideoSettings(this, _options));
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
