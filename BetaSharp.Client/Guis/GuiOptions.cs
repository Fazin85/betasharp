using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiOptions : GuiScreen
{
    private readonly GuiScreen _parentScreen;
    private readonly GameOptions _options;

    public GuiOptions(GuiScreen parentScreen, GameOptions options)
    {
        _parentScreen = parentScreen;
        _options = options;

        TranslationStorage translations = TranslationStorage.Instance;
        Text = translations.TranslateKey("options.title");
        DisplayTitle = true;

        int buttonLeft = Width / 2 - 100;
        int topY = Height / 6 + 12;

        for (int i = 0; i < _options.MainScreenOptions.Length; i++)
        {
            GameOption option = _options.MainScreenOptions[i];
            int x = buttonLeft - 55 + (i % 2 * 160);
            int y = topY + 12 * (i / 2);

            switch (option)
            {
                case FloatOption floatOpt:
                    Children.Add(new Slider(x, y, option.GetDisplayString(translations), floatOpt.Set,
                        floatOpt.Value, floatOpt.Min, floatOpt.Max, floatOpt.Step));
                    break;
                case BoolOption boolOpt:
                    Children.Add(new ToggleButton(x, y, boolOpt, option.GetDisplayString(translations)));
                    break;
                case CycleOption cycleOpt:
                    Children.Add(new CycleButton(x, y, cycleOpt, option.GetDisplayString(translations)));
                    break;
            }
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
        Children.AddRange([videoSettingsButton, debugSettingsButton, audioSettingsButton, controlsButton, doneButton]);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
