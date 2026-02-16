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

    public override void initGui()
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
                controlList.add(new GuiSmallButton(currentOption.returnEnumOrdinal(), width / 2 - 155 + rowIndex % 2 * 160, height / 6 + 24 * (rowIndex >> 1), currentOption, options.getKeyBinding(currentOption)));
            }
            else
            {
                controlList.add(new GuiSlider(currentOption.returnEnumOrdinal(), width / 2 - 155 + rowIndex % 2 * 160, height / 6 + 24 * (rowIndex >> 1), currentOption, options.getKeyBinding(currentOption), options.getOptionFloatValue(currentOption)));
            }

            ++rowIndex;
        }

        controlList.add(new GuiButton(BUTTON_VIDEO_SETTINGS, width / 2 - 100, height / 6 + 96 + 12, translations.translateKey("options.video")));
        controlList.add(new GuiButton(BUTTON_CONTROLS, width / 2 - 100, height / 6 + 120 + 12, translations.translateKey("options.controls")));
        controlList.add(new GuiButton(BUTTON_DONE, width / 2 - 100, height / 6 + 168, translations.translateKey("gui.done")));
    }

    protected override void actionPerformed(GuiButton button)
    {
        if (button.enabled)
        {
            if (button.id < 100 && button is GuiSmallButton)
            {
                options.setOptionValue(((GuiSmallButton)button).returnEnumOptions(), 1);
                button.displayString = options.getKeyBinding(EnumOptions.getEnumOptions(button.id));
            }

            switch (button.id)
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

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, screenTitle, width / 2, 20, 0x00FFFFFF);
        base.render(mouseX, mouseY, partialTicks);
    }
}
