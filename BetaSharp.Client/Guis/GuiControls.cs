using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiControls : GuiScreen
{

    private readonly GuiScreen parentScreen;
    protected string screenTitle = "Controls";
    private readonly GameOptions options;
    private int selectedKey = -1;
    private const int BUTTON_DONE = 200;

    public GuiControls(GuiScreen var1, GameOptions var2)
    {
        parentScreen = var1;
        options = var2;
    }

    private int getLeftColumnX()
    {
        return Width / 2 - 155;
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        int leftX = getLeftColumnX();

        for (int i = 0; i < options.keyBindings.Length; ++i)
        {
            controlList.Add(new GuiSmallButton(i, leftX + i % 2 * 160, Height / 6 + 24 * (i >> 1), 70, 20, options.getOptionDisplayString(i)));
        }

        controlList.Add(new GuiButton(BUTTON_DONE, Width / 2 - 100, Height / 6 + 168, translations.translateKey("gui.done")));
        screenTitle = translations.translateKey("controls.title");
    }

    protected override void ActionPerformed(GuiButton button)
    {
        for (int i = 0; i < options.keyBindings.Length; ++i)
        {
            controlList[i].DisplayString = options.getOptionDisplayString(i);
        }

        switch (button.Id)
        {
            case BUTTON_DONE:
                mc.displayGuiScreen(parentScreen);
                break;
            default:
                selectedKey = button.Id;
                button.DisplayString = "> " + options.getOptionDisplayString(button.Id) + " <";
                break;
        }

    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        if (selectedKey >= 0)
        {
            options.setKeyBinding(selectedKey, eventKey);
            controlList[selectedKey].DisplayString = options.getOptionDisplayString(selectedKey);
            selectedKey = -1;
        }
        else
        {
            base.KeyTyped(eventChar, eventKey);
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, screenTitle, Width / 2, 20, 0x00FFFFFF);
        int leftX = getLeftColumnX();

        for (int i = 0; i < options.keyBindings.Length; ++i)
        {
            DrawString(fontRenderer, options.getKeyBindingDescription(i), leftX + i % 2 * 160 + 70 + 6, Height / 6 + 24 * (i >> 1) + 7, 0xFFFFFFFF);
        }

        base.Render(mouseX, mouseY, partialTicks);
    }
}
