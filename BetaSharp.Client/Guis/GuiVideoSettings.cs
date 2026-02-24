using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiVideoSettings : GuiScreen
{
    private readonly GuiScreen _parentScreen;
    private readonly GameOptions _options;

    public GuiVideoSettings(GuiScreen parent, GameOptions options)
    {
        _parentScreen = parent;
        _options = options;
        DisplayTitle = true;

        TranslationStorage translations = TranslationStorage.Instance;
        Text = translations.TranslateKey("options.videoTitle");
        DisplayTitle = true;

        int buttonLeft = Width / 2 - 100;
        int topY = Height / 6 + 12;

        for (int i = 0; i < _options.MainScreenOptions.Length; i++)
        {
            GameOption option = _options.MainScreenOptions[i];
            int x = buttonLeft - 55 + (i % 2 * 160);
            int y = topY + 12 * (i >> 1);

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
                    if (option == _options.GuiScaleOption)
                    {
                        Children[^1].Clicked += (_, _) =>
                        {
                            ScaledResolution scaled = new(mc.options, mc.displayWidth, mc.displayHeight);
                            int scaledWidth = scaled.ScaledWidth;
                            int scaledHeight = scaled.ScaledHeight;
                            SetWorldAndResolution(mc, scaledWidth, scaledHeight);
                        };
                    }
                    break;
            }
        }

        Button doneButton = new(buttonLeft, topY + 156, translations.TranslateKey("gui.done"));
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
