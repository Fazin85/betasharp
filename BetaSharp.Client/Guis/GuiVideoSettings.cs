using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiVideoSettings : GuiScreen
{

    private readonly GuiScreen _parentScreen;
    protected string _screenTitle = "Video Settings";
    private readonly GameOptions _gameOptions;
    private static readonly EnumOptions[] _videoOptions = { EnumOptions.RENDER_DISTANCE, EnumOptions.FOV, EnumOptions.FRAMERATE_LIMIT, EnumOptions.VIEW_BOBBING, EnumOptions.GUI_SCALE, EnumOptions.ANISOTROPIC, EnumOptions.MIPMAPS, EnumOptions.MSAA, EnumOptions.ENVIRONMENT_ANIMATION, EnumOptions.DEBUG_MODE };

    public GuiVideoSettings(GuiScreen parent, GameOptions options)
    {
        _parentScreen = parent;
        _gameOptions = options;
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        _screenTitle = translations.translateKey("options.videoTitle");
        int optionIndex = 0;

        foreach (EnumOptions option in _videoOptions)
        {
            int x = Width / 2 - 155 + (optionIndex % 2) * 160;
            int y = Height / 6 + 24 * (optionIndex / 2);
            int id = option.returnEnumOrdinal();

            if (!option.getEnumFloat())
            {
                // Toggle-style button (e.g., Fancy/Fast or On/Off)
                _controlList.Add(new GuiSmallButton(id, x, y, option, _gameOptions.getKeyBinding(option)));
            }
            else
            {
                // Slider-style button (e.g., FOV or Render Distance)
                _controlList.Add(new GuiSlider(id, x, y, option, _gameOptions.getKeyBinding(option), _gameOptions.getOptionFloatValue(option)));
            }

            optionIndex++;
        }

        _controlList.Add(new GuiButton(200, Width / 2 - 100, Height / 6 + 168, translations.translateKey("gui.done")));
    }

    protected override void ActionPerformed(GuiButton btn)
    {
        if (btn.Enabled)
        {
            if (btn.Id < 100 && btn is GuiSmallButton)
            {
                _gameOptions.setOptionValue(((GuiSmallButton)btn).returnEnumOptions(), 1);
                btn.DisplayString = _gameOptions.getKeyBinding(EnumOptions.getEnumOptions(btn.Id));
            }

            if (btn.Id == 200)
            {
                mc.options.saveOptions();
                mc.displayGuiScreen(_parentScreen);
            }

            if (btn.Id == (int)EnumOptions.GUI_SCALE.ordinal())
            {
                ScaledResolution scaled = new(mc.options, mc.displayWidth, mc.displayHeight);
                int scaledWidth = scaled.ScaledWidth;
                int scaledHeight = scaled.ScaledHeight;
                SetWorldAndResolution(mc, scaledWidth, scaledHeight);
            }
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(FontRenderer, _screenTitle, Width / 2, 20, 0xFFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }
}
