namespace BetaSharp.Client.Guis;

public class GuiConnectFailed : GuiScreen
{

    private readonly string errorMessage;
    private readonly string errorDetail;
    private const int BUTTON_TO_MENU = 0;

    public GuiConnectFailed(string messageKey, string detailKey, params object[] formatArgs)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        errorMessage = translations.translateKey(messageKey);
        if (formatArgs != null)
        {
            errorDetail = translations.translateKeyFormat(detailKey, formatArgs);
        }
        else
        {
            errorDetail = translations.translateKey(detailKey);
        }

    }

    public override void updateScreen()
    {
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
    }

    public override void initGui()
    {
        mc.stopInternalServer();
        TranslationStorage translations = TranslationStorage.getInstance();
        controlList.clear();
        controlList.add(new GuiButton(BUTTON_TO_MENU, width / 2 - 100, height / 4 + 120 + 12, translations.translateKey("gui.toMenu")));
    }

    protected override void actionPerformed(GuiButton var1)
    {
        switch (var1.id)
        {
            case BUTTON_TO_MENU:
                mc.displayGuiScreen(new GuiMainMenu());
                break;
        }

    }

    public override void render(int var1, int var2, float var3)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, errorMessage, width / 2, height / 2 - 50, 0x00FFFFFF);
        drawCenteredString(fontRenderer, errorDetail, width / 2, height / 2 - 10, 0x00FFFFFF);
        base.render(var1, var2, var3);
    }
}
