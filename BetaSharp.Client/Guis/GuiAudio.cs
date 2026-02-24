using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiAudio : GuiScreen
{

    private readonly GuiScreen _parentScreen;
    private readonly GameOptions _gameOptions;

    public GuiAudio(GuiScreen parent, GameOptions options)
    {
        _parentScreen = parent;
        _gameOptions = options;


        TranslationStorage translations = TranslationStorage.Instance;
        Text = "Audio Settings";
        DisplayTitle = true;
        int optionIndex = 0;

        foreach (GameOption option in _gameOptions.AudioScreenOptions)
        {
            int x = Width / 2 - 155 + (optionIndex % 2) * 160;
            int y = Height / 6 + 24 * (optionIndex / 2);
            int id = optionIndex;

            if (option is FloatOption floatOpt)
            {
                Children.Add(new GuiOptionsSlider(id, x, y, floatOpt, option.GetDisplayString(translations), floatOpt.Value));
            }
            else
            {
                Children.Add(new GuiSmallButton(id, x, y, option, option.GetDisplayString(translations)));
            }

            optionIndex++;
        }

        Children.Add(new GuiButton(200, Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done")));

    }

    protected override void ActionPerformed(GuiButton btn)
    {
        if (btn.Enabled)
        {
            if (btn is GuiSmallButton smallBtn && smallBtn.Option != null)
            {
                smallBtn.ClickOption();
                btn.DisplayString = smallBtn.Option.GetDisplayString(TranslationStorage.Instance);
            }

            if (btn.Id == 200)
            {
                mc.options.SaveOptions();
                mc.OpenScreen(_parentScreen);
            }
        }
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
