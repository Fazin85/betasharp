namespace BetaSharp.Client.Guis;

public class GuiConnectFailed: GuiScreen
{
    private readonly string _errorMessage;
    private readonly string _errorDetail;
    private const int _buttonToMenu = 0;

    public GuiConnectFailed(string messageKey, string detailKey, params object[]? formatArgs)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        _errorMessage = translations.TranslateKey(messageKey);
        if (formatArgs != null)
        {
            _errorDetail = translations.TranslateKeyFormat(detailKey, formatArgs);
        }
        else
        {
            _errorDetail = translations.TranslateKey(detailKey);
        }

    }

    public override void UpdateScreen()
    {
    }

    protected override void OnKeyInput(KeyboardEventArgs e)
    {
    }

    public override void InitGui()
    {
        mc.stopInternalServer();
        TranslationStorage translations = TranslationStorage.Instance;
        Children.Clear();
        Children.Add(new Button(_buttonToMenu, Width / 2 - 100, Height / 4 + 120 + 12, translations.TranslateKey("gui.toMenu")));
    }

    protected override void ActionPerformed(Button btt)
    {
        switch (btt.Id)
        {
            case _buttonToMenu:
                mc.OpenScreen(new GuiMainMenu());
                break;
        }

    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
        Gui.DrawCenteredString(FontRenderer, _errorMessage, Width / 2, Height / 2 - 50, 0xFFFFFF);
        Gui.DrawCenteredString(FontRenderer, _errorDetail, Width / 2, Height / 2 - 10, 0xFFFFFF);
        base.OnRendered(mouseX, mouseY, tickDelta);
    }
}
