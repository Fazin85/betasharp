namespace BetaSharp.Client.Guis;

public class GuiYesNo : GuiScreen
{
    private const int ButtonConfirm = 0;
    private const int ButtonCancel = 1;

    private readonly GuiScreen _parentScreen;
    private readonly string _message1;
    private readonly string _message2;
    private readonly string _confirmButtonText;
    private readonly string _cancelButtonText;
    private readonly int _worldNumber;

    public GuiYesNo(GuiScreen parentScreen, string message1, string message2, string confirmButtonText, string cancelButtonText, int worldNumber)
    {
        _parentScreen = parentScreen;
        _message1 = message1;
        _message2 = message2;
        _confirmButtonText = confirmButtonText;
        _cancelButtonText = cancelButtonText;
        _worldNumber = worldNumber;
    }

    public override void InitGui()
    {
        _controlList.Add(new GuiSmallButton(ButtonConfirm, Width / 2 - 155 + 0, Height / 6 + 96, _confirmButtonText));
        _controlList.Add(new GuiSmallButton(ButtonCancel, Width / 2 - 155 + 160, Height / 6 + 96, _cancelButtonText));
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case ButtonConfirm:
                _parentScreen.DeleteWorld(true, _worldNumber);
                break;
            case ButtonCancel:
                _parentScreen.DeleteWorld(false, _worldNumber);
                break;
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(FontRenderer, _message1, Width / 2, 70, 0x00FFFFFF);
        DrawCenteredString(FontRenderer, _message2, Width / 2, 90, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }
}