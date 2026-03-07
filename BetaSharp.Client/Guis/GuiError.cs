namespace BetaSharp.Client.Guis;

public class GuiError : GuiScreen
{
    private readonly string _title;
    private readonly string _message;
    private readonly GuiScreen _parent;

    public GuiError(GuiScreen parent, string title, string message)
    {
        _parent = parent;
        _title = title;
        _message = message;
    }

    public override void InitGui()
    {
        _controlList.Clear();
        TranslationStorage translations = TranslationStorage.Instance;
        _controlList.Add(new GuiButton(0, Width / 2 - 100, Height / 4 + 120 + 12, translations.TranslateKey("gui.back")));
    }

    protected override void ActionPerformed(GuiButton btt)
    {
        if (btt.Id == 0)
        {
            Game.displayGuiScreen(_parent);
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(FontRenderer, _title, Width / 2, Height / 2 - 50, Color.White);
        DrawCenteredString(FontRenderer, _message, Width / 2, Height / 2 - 10, Color.White);
        base.Render(mouseX, mouseY, partialTicks);
    }
}
