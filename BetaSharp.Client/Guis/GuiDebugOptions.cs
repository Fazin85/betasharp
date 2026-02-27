using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiDebugOptions : Screen
{
    public GuiDebugOptions(Screen parent, GameOptions options)
    {
        Text = "Debug Options";
        DisplayTitle = true;

        TranslationStorage translations = TranslationStorage.Instance;

        for (int i = 0; i < options.DebugScreenOptions.Length; i++)
        {
            GameOption option = options.DebugScreenOptions[i];
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
            else if (option is CycleOption cycleOpt)
            {
                AddChild(new CycleButton(x, y, cycleOpt));
            }
        }

        Button doneButton = new(Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done"));
        doneButton.Clicked += (_, _) =>
        {
            options.SaveOptions();
            MC.OpenScreen(parent);
        };
        AddChild(doneButton);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
