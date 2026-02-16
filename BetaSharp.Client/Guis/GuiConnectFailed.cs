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

    public override void UpdateScreen()
    {
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }

    public override void InitGui()
    {
        mc.stopInternalServer();
        TranslationStorage translations = TranslationStorage.getInstance();
        controlList.Clear();
        controlList.Add(new GuiButton(BUTTON_TO_MENU, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.toMenu")));
    }

    protected override void ActionPerformed(GuiButton var1)
    {
        switch (var1.Id)
        {
            case BUTTON_TO_MENU:
                mc.displayGuiScreen(new GuiMainMenu());
                break;
        }

    }

    public override void Render(int var1, int var2, float var3)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, errorMessage, Width / 2, Height / 2 - 50, 0x00FFFFFF);
        DrawCenteredString(fontRenderer, errorDetail, Width / 2, Height / 2 - 10, 0x00FFFFFF);
        base.Render(var1, var2, var3);
    }
}
