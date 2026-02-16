namespace BetaSharp.Client.Guis;

public class GuiUnused : GuiScreen
{

    private readonly string message1;
    private readonly string message2;

    public override void InitGui()
    {
    }

    public override void Render(int var1, int var2, float var3)
    {
        DrawGradientRect(0, 0, Width, Height, 0xFF402020, 0xFF501010);
        DrawCenteredString(fontRenderer, message1, Width / 2, 90, 0x00FFFFFF);
        DrawCenteredString(fontRenderer, message2, Width / 2, 110, 0x00FFFFFF);
        base.Render(var1, var2, var3);
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }
}