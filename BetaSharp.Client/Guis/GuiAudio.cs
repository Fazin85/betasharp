using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiAudio : Screen
{
    private readonly GameOptions _gameOptions;

    public GuiAudio(Screen parent, GameOptions options)
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
                AddChild(new OptionsSlider(x, y, floatOpt));
            }
            else if (option is BoolOption boolOpt)
            {
                AddChild(new ToggleButton(x, y, boolOpt));
            }
        }

        Button doneButton = new(Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done"));
        doneButton.Clicked += (_, _) =>
        {
            _gameOptions.SaveOptions();
            MC.OpenScreen(parent);
        };
        AddChild(doneButton);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
