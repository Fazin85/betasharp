using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiVideoSettings : GuiScreen
{
    private readonly GuiScreen _parentScreen;
    private readonly GameOptions _gameOptions;

    public GuiVideoSettings(GuiScreen parent, GameOptions options)
    {
        _parentScreen = parent;
        _gameOptions = options;
        DisplayTitle = true;

        TranslationStorage translations = TranslationStorage.Instance;
        Text = translations.TranslateKey("options.videoTitle");
        int optionIndex = 0;

        foreach (GameOption option in _gameOptions.VideoScreenOptions)
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

            if (btn is GuiSmallButton { Option: CycleOption } guiScaleBtn
                && guiScaleBtn.Option == _gameOptions.GuiScaleOption)
            {
                ScaledResolution scaled = new(mc.options, mc.displayWidth, mc.displayHeight);
                int scaledWidth = scaled.ScaledWidth;
                int scaledHeight = scaled.ScaledHeight;
                SetWorldAndResolution(mc, scaledWidth, scaledHeight);
            }
        }
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
    }
}
