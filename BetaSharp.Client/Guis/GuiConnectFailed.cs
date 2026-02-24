namespace BetaSharp.Client.Guis;

public class GuiConnectFailed : GuiScreen
{
    private readonly string _errorMessage;
    private readonly string _errorDetail;

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

        mc.stopInternalServer();
        Button titleButton = new(Width / 2 - 100, Height / 4 + 120 + 12, translations.TranslateKey("gui.toMenu"));
        titleButton.Clicked += (_, _) => mc.OpenScreen(new GuiMainMenu());
        Children.Add(titleButton);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
        Gui.DrawCenteredString(FontRenderer, _errorMessage, Width / 2, Height / 2 - 50, 0xFFFFFF);
        Gui.DrawCenteredString(FontRenderer, _errorDetail, Width / 2, Height / 2 - 10, 0xFFFFFF);
    }
}
