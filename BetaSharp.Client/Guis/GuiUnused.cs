namespace BetaSharp.Client.Guis;

public class GuiUnused : GuiScreen
{

    private readonly string _primaryMessage;
    private readonly string _secondaryMessage;

    public override void InitGui()
    {
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawGradientRect(0, 0, Width, Height, 0xFF402020, 0xFF501010);
        DrawCenteredString(FontRenderer, _primaryMessage, Width / 2, 90, 0x00FFFFFF);
        DrawCenteredString(FontRenderer, _secondaryMessage, Width / 2, 110, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }
}