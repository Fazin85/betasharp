namespace BetaSharp.Client.Guis;

public class GuiYesNo : GuiScreen
{

    private readonly GuiScreen parentScreen;
    private readonly string message1;
    private readonly string message2;
    private readonly string confirmButtonText;
    private readonly string cancelButtonText;
    private readonly int worldNumber;

    private const int BUTTON_CONFIRM = 0;
    private const int BUTTON_CANCEL = 1;

    public GuiYesNo(GuiScreen parentScreen, string message1, string message2, string confirmButtonText, string cancelButtonText, int worldNumber)
    {
        this.parentScreen = parentScreen;
        this.message1 = message1;
        this.message2 = message2;
        this.confirmButtonText = confirmButtonText;
        this.cancelButtonText = cancelButtonText;
        this.worldNumber = worldNumber;
    }

    public override void InitGui()
    {
        controlList.Add(new GuiSmallButton(BUTTON_CONFIRM, Width / 2 - 155 + 0, Height / 6 + 96, confirmButtonText));
        controlList.Add(new GuiSmallButton(BUTTON_CANCEL, Width / 2 - 155 + 160, Height / 6 + 96, cancelButtonText));
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case BUTTON_CONFIRM:
                parentScreen.DeleteWorld(true, worldNumber);
                break;
            case BUTTON_CANCEL:
                parentScreen.DeleteWorld(false, worldNumber);
                break;
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, message1, Width / 2, 70, 0x00FFFFFF);
        DrawCenteredString(fontRenderer, message2, Width / 2, 90, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }
}