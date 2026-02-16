using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiOptions : GuiScreen
{
    private const int BUTTON_VIDEO_SETTINGS = 101;
    private const int BUTTON_CONTROLS = 100;
    private const int BUTTON_DONE = 200;

    private readonly GuiScreen parentScreen;
    protected string screenTitle = "Options";
    private readonly GameOptions options;
    private static readonly EnumOptions[] availableOptions = new EnumOptions[] { EnumOptions.MUSIC, EnumOptions.SOUND, EnumOptions.INVERT_MOUSE, EnumOptions.SENSITIVITY, EnumOptions.DIFFICULTY };

    public GuiOptions(GuiScreen parentScreen, GameOptions gameOptions)
    {
        this.parentScreen = parentScreen;
        this.options = gameOptions;
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        screenTitle = translations.translateKey("options.title");
        int rowIndex = 0;
        EnumOptions[] optionsToDisplay = availableOptions;
        int optionsLength = optionsToDisplay.Length;

        for (int i = 0; i < optionsLength; ++i)
        {
            EnumOptions currentOption = optionsToDisplay[i];
            if (!currentOption.getEnumFloat())
            {
                controlList.Add(new GuiSmallButton(currentOption.returnEnumOrdinal(), Width / 2 - 155 + rowIndex % 2 * 160, Height / 6 + 24 * (rowIndex >> 1), currentOption, options.getKeyBinding(currentOption)));
            }
            else
            {
                controlList.Add(new GuiSlider(currentOption.returnEnumOrdinal(), Width / 2 - 155 + rowIndex % 2 * 160, Height / 6 + 24 * (rowIndex >> 1), currentOption, options.getKeyBinding(currentOption), options.getOptionFloatValue(currentOption)));
            }

            ++rowIndex;
        }

        controlList.Add(new GuiButton(BUTTON_VIDEO_SETTINGS, Width / 2 - 100, Height / 6 + 96 + 12, translations.translateKey("options.video")));
        controlList.Add(new GuiButton(BUTTON_CONTROLS, Width / 2 - 100, Height / 6 + 120 + 12, translations.translateKey("options.controls")));
        controlList.Add(new GuiButton(BUTTON_DONE, Width / 2 - 100, Height / 6 + 168, translations.translateKey("gui.done")));
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (button.Enabled)
        {
            if (button.Id < 100 && button is GuiSmallButton)
            {
                options.setOptionValue(((GuiSmallButton)button).returnEnumOptions(), 1);
                button.DisplayString = options.getKeyBinding(EnumOptions.getEnumOptions(button.Id));
            }

            switch (button.Id)
            {
                case BUTTON_VIDEO_SETTINGS:
                    mc.options.saveOptions();
                    mc.displayGuiScreen(new GuiVideoSettings(this, options));
                    break;
                case BUTTON_CONTROLS:
                    mc.options.saveOptions();
                    mc.displayGuiScreen(new GuiControls(this, options));
                    break;
                case BUTTON_DONE:
                    mc.options.saveOptions();
                    mc.displayGuiScreen(parentScreen);
                    break;
            }
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, screenTitle, Width / 2, 20, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }
}
