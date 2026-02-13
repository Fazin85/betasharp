namespace BetaSharp.Client.Guis;

public class GuiYesNo : GuiScreen
{

    private GuiScreen parentScreen;
    private string message1;
    private string message2;
    private string confirmButtonText;
    private string cancelButtonText;
    private int worldNumber;

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

    public override void initGui()
    {
        controlList.add(new GuiSmallButton(BUTTON_CONFIRM, width / 2 - 155 + 0, height / 6 + 96, confirmButtonText));
        controlList.add(new GuiSmallButton(BUTTON_CANCEL, width / 2 - 155 + 160, height / 6 + 96, cancelButtonText));
    }

    protected override void actionPerformed(GuiButton button)
    {
        switch (button.id)
        {
            case BUTTON_CONFIRM:
                parentScreen.deleteWorld(true, worldNumber);
                break;
            case BUTTON_CANCEL:
                parentScreen.deleteWorld(false, worldNumber);
                break;
        }
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, message1, width / 2, 70, 16777215);
        drawCenteredString(fontRenderer, message2, width / 2, 90, 16777215);
        base.render(mouseX, mouseY, partialTicks);
    }
}