using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiAudio : GuiScreen
{
    private readonly GameOptions _gameOptions;

    public GuiAudio(GuiScreen parent, GameOptions options)
    {
        _gameOptions = options;

        TranslationStorage translations = TranslationStorage.Instance;
        Text = "Audio Settings";
        DisplayTitle = true;

        for (int i = 0; i < _gameOptions.AudioScreenOptions.Length; i++)
        {
            GameOption option = _gameOptions.AudioScreenOptions[i];
            int x = Width / 2 - 155 + (i % 2) * 160;
            int y = Height / 6 + 24 * (i / 2);

            if (option is FloatOption floatOpt)
            {
                Children.Add(new OptionsSlider(x, y, floatOpt, option.GetDisplayString(translations), floatOpt.Value, 0,
                    100, 1));
            }
            else if (option is BoolOption boolOpt)
            {
                Children.Add(new ToggleButton(x, y, boolOpt, option.GetDisplayString(translations)));
            }
        }

        Button doneButton = new(Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done"));
        doneButton.Clicked += (_, _) =>
        {
            _gameOptions.SaveOptions();
            mc.OpenScreen(parent);
        };
        Children.Add(doneButton);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
