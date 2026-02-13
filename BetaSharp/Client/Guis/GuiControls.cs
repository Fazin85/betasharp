using BetaSharp.Client.Resource.Language;

namespace BetaSharp.Client.Guis;

public class GuiControls : GuiScreen
{

    private GuiScreen parentScreen;
    protected string screenTitle = "Controls";
    private GameOptions options;
    private int selectedKey = -1;
    private const int BUTTON_DONE = 200;

    public GuiControls(GuiScreen var1, GameOptions var2)
    {
        parentScreen = var1;
        options = var2;
    }

    private int getLeftColumnX()
    {
        return width / 2 - 155;
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        int leftX = getLeftColumnX();

        for (int i = 0; i < options.keyBindings.Length; ++i)
        {
            controlList.add(new GuiSmallButton(i, leftX + i % 2 * 160, height / 6 + 24 * (i >> 1), 70, 20, options.getOptionDisplayString(i)));
        }

        controlList.add(new GuiButton(BUTTON_DONE, width / 2 - 100, height / 6 + 168, translations.translateKey("gui.done")));
        screenTitle = translations.translateKey("controls.title");
    }

    protected override void actionPerformed(GuiButton button)
    {
        for (int i = 0; i < options.keyBindings.Length; ++i)
        {
            ((GuiButton)controlList.get(i)).displayString = options.getOptionDisplayString(i);
        }

        switch (button.id)
        {
            case BUTTON_DONE:
                mc.displayGuiScreen(parentScreen);
                break;
            default:
                selectedKey = button.id;
                button.displayString = "> " + options.getOptionDisplayString(button.id) + " <";
                break;
        }

    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
        if (selectedKey >= 0)
        {
            options.setKeyBinding(selectedKey, eventKey);
            ((GuiButton)controlList.get(selectedKey)).displayString = options.getOptionDisplayString(selectedKey);
            selectedKey = -1;
        }
        else
        {
            base.keyTyped(eventChar, eventKey);
        }

    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, screenTitle, width / 2, 20, 16777215);
        int leftX = getLeftColumnX();

        for (int i = 0; i < options.keyBindings.Length; ++i)
        {
            drawString(fontRenderer, options.getKeyBindingDescription(i), leftX + i % 2 * 160 + 70 + 6, height / 6 + 24 * (i >> 1) + 7, -1);
        }

        base.render(mouseX, mouseY, partialTicks);
    }
}