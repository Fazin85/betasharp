using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiOptions : GuiScreen
{
    private readonly GuiScreen _parentScreen;
    private readonly GameOptions _options;

    public GuiOptions(GuiScreen parentScreen, GameOptions gameOptions)
    {
        _parentScreen = parentScreen;
        _options = gameOptions;

        TranslationStorage translations = TranslationStorage.Instance;
        Text = translations.TranslateKey("options.title");
        DisplayTitle = true;
        int rowIndex = 0;

        int buttonLeft = Width / 2 - 100;
        int topY = Height / 6 + 12;

        foreach (GameOption option in _options.MainScreenOptions)
        {
            int xPos = buttonLeft - 55 + (rowIndex % 2 * 160);
            int yPos = topY + 12 * (rowIndex >> 1);

            if (option is FloatOption floatOpt)
            {
                Children.Add(new GuiSlider(xPos, yPos, option.GetDisplayString(translations), floatOpt.Set,
                    floatOpt.Value, floatOpt.Min, floatOpt.Max, floatOpt.Step));
            }
            else if (option is BoolOption boolOpt)
            {
                Children.Add(new ToggleButton(xPos, yPos, boolOpt, option.GetDisplayString(translations)));
            }
            else if (option is CycleOption cycleOpt)
            {
                Children.Add(new CycleButton(xPos, yPos, cycleOpt, option.GetDisplayString(translations)));
            }

            ++rowIndex;
        }

        GuiButton videoSettingsButton = new(buttonLeft - 55, topY + 72, translations.TranslateKey("options.video"));
        GuiButton debugSettingsButton = new(buttonLeft + 105, topY + 72, "Debug Settings...");
        GuiButton audioSettingsButton = new(buttonLeft - 55, topY + 96, "Audio Settings");
        GuiButton controlsButton = new(buttonLeft + 105, topY + 96, translations.TranslateKey("options.controls"));
        GuiButton doneButton = new(buttonLeft, topY + 168, translations.TranslateKey("gui.done"));
        videoSettingsButton.Clicked += (_, _) =>
        {
            mc.options.SaveOptions();
            mc.OpenScreen(new GuiVideoSettings(this, _options));
        };
        debugSettingsButton.Clicked += (_, _) =>
        {
            mc.options.SaveOptions();
            mc.OpenScreen(new GuiDebugOptions(this, _options));
        };
        audioSettingsButton.Clicked += (_, _) =>
        {
            mc.options.SaveOptions();
            mc.OpenScreen(new GuiAudio(this, _options));
        };
        controlsButton.Clicked += (_, _) =>
        {
            mc.options.SaveOptions();
            mc.OpenScreen(new GuiControls(this, _options));
        };
        doneButton.Clicked += (_, _) =>
        {
            mc.options.SaveOptions();
            mc.OpenScreen(_parentScreen);
        };
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
