using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiOptions : Screen
{
    public GuiOptions(Screen parentScreen, GameOptions options)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        Text = translations.TranslateKey("options.title");
        DisplayTitle = true;

        int buttonLeft = Width / 2 - 100;
        int topY = Height / 6 + 12;

        for (int i = 0; i < options.MainScreenOptions.Length; i++)
        {
            GameOption option = options.MainScreenOptions[i];
            int x = buttonLeft - 55 + (i % 2 * 160);
            int y = topY + 12 * (i / 2);

            switch (option)
            {
                case FloatOption floatOpt:
                    AddChild(new OptionsSlider(x, y, floatOpt));
                    break;
                case BoolOption boolOpt:
                    AddChild(new ToggleButton(x, y, boolOpt));
                    break;
                case CycleOption cycleOpt:
                    AddChild(new CycleButton(x, y, cycleOpt));
                    break;
            }
        }

        Button videoSettingsButton = new(buttonLeft - 55, topY + 72, translations.TranslateKey("options.video"));
        Button debugSettingsButton = new(buttonLeft + 105, topY + 72, "Debug Settings...");
        Button audioSettingsButton = new(buttonLeft - 55, topY + 96, "Audio Settings");
        Button controlsButton = new(buttonLeft + 105, topY + 96, translations.TranslateKey("options.controls"));
        Button doneButton = new(buttonLeft, topY + 168, translations.TranslateKey("gui.done"));
        videoSettingsButton.Clicked += (_, _) =>
        {
            MC.options.SaveOptions();
            MC.OpenScreen(new GuiVideoSettings(this, options));
        };
        debugSettingsButton.Clicked += (_, _) =>
        {
            MC.options.SaveOptions();
            MC.OpenScreen(new GuiDebugOptions(this, options));
        };
        audioSettingsButton.Clicked += (_, _) =>
        {
            MC.options.SaveOptions();
            MC.OpenScreen(new GuiAudio(this, options));
        };
        controlsButton.Clicked += (_, _) =>
        {
            MC.options.SaveOptions();
            MC.OpenScreen(new GuiControls(this, options));
        };
        doneButton.Clicked += (_, _) =>
        {
            MC.options.SaveOptions();
            MC.OpenScreen(parentScreen);
        };
        AddChildren(videoSettingsButton, debugSettingsButton, audioSettingsButton, controlsButton, doneButton);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
