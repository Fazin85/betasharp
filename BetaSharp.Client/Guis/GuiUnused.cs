namespace BetaSharp.Client.Guis;

public class GuiUnused : GuiScreen
{

    private readonly string message1;
    private readonly string message2;

    public override void initGui()
    {
    }

    public override void render(int var1, int var2, float var3)
    {
        drawGradientRect(0, 0, width, height, 0xFF402020, 0xFF501010);
        drawCenteredString(fontRenderer, message1, width / 2, 90, 0x00FFFFFF);
        drawCenteredString(fontRenderer, message2, width / 2, 110, 0x00FFFFFF);
        base.render(var1, var2, var3);
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
    }
}