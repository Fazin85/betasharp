using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiControls : Screen
{
    private readonly GameOptions _options;

    public GuiControls(Screen parentScreen, GameOptions options)
    {
        _options = options;

        TranslationStorage translations = TranslationStorage.Instance;
        int leftX = GetLeftColumnX();

        for (int i = 0; i < _options.KeyBindings.Length; ++i)
        {
            Children.Add(new ControlsButton(leftX + i % 2 * 160, Height / 6 + 24 * (i >> 1), _options.KeyBindings[i]));
        }

        OptionsSlider sensitivitySlider = new(Width / 2 + 5, Height / 6 + 130, _options.MouseSensitivityOption,
            _options.MouseSensitivityOption.GetDisplayString(translations), _options.MouseSensitivityOption.Value, 0.2f,
            0.8f) { Size = new(125, 20) };
        ToggleButton invertMouseButton = new(Width / 2 - 155, Height / 6 + 130, _options.InvertMouseOption,
            _options.InvertMouseOption.GetDisplayString(translations)) { Size = new(125, 20) };
        Button doneButton = new(Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done"));
        doneButton.Clicked += (_, _) =>
        {
            _options.SaveOptions();
            MC.OpenScreen(parentScreen);
        };

        Children.AddRange(sensitivitySlider, invertMouseButton, doneButton);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
        int leftX = GetLeftColumnX();

        for (int i = 0; i < _options.KeyBindings.Length; ++i)
        {
            Gui.DrawString(FontRenderer, _options.GetKeyBindingDescription(i), leftX + i % 2 * 160 + 70 + 6, Height / 6 + 24 * (i >> 1) + 7, 0xFFFFFFFF);
        }
    }

    private int GetLeftColumnX() => Width / 2 - 155;
}
