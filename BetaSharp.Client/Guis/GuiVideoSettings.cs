using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiVideoSettings : Screen
{
    private readonly Screen _parentScreen;
    private readonly GameOptions _options;

    public GuiVideoSettings(Screen parent, GameOptions options)
    {
        _parentScreen = parent;
        _options = options;
        DisplayTitle = true;

        TranslationStorage translations = TranslationStorage.Instance;
        Text = translations.TranslateKey("options.videoTitle");
        DisplayTitle = true;

        int buttonLeft = Width / 2 - 100;
        int topY = Height / 6;

        for (int i = 0; i < _options.VideoScreenOptions.Length; i++)
        {
            GameOption option = _options.VideoScreenOptions[i];
            int x = buttonLeft - 55 + (i % 2 * 160);
            int y = topY + (24 * (i / 2));

            switch (option)
            {
                case FloatOption floatOpt:
                    AddChild(new OptionsSlider(x, y, floatOpt));
                    break;
                case BoolOption boolOpt:
                    AddChild(new ToggleButton(x, y, boolOpt));
                    break;
                case CycleOption cycleOpt:
                    var cycleButton = new CycleButton(x, y, cycleOpt);
                    AddChild(cycleButton);
                    if (option == _options.GuiScaleOption)
                    {
                        cycleButton.Clicked += (_, _) =>
                        {
                            ScaledResolution scaled = new(MC.options, MC.displayWidth, MC.displayHeight);
                            int scaledWidth = scaled.ScaledWidth;
                            int scaledHeight = scaled.ScaledHeight;
                            SetWorldAndResolution(MC, scaledWidth, scaledHeight);
                        };
                    }
                    break;
            }
        }

        Button doneButton = new(buttonLeft, topY + 168, translations.TranslateKey("gui.done"));
        doneButton.Clicked += (_, _) =>
        {
            MC.options.SaveOptions();
            MC.OpenScreen(_parentScreen);
        };

        AddChild(doneButton);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
