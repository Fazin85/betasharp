using BetaSharp.Client.Input;

namespace BetaSharp.Client.Guis;

public class GuiScreenAddServer : GuiScreen
{
    private readonly GuiMultiplayer _parentScreen;
    private TextField _serverName = null!;
    private TextField _serverAddress = null!;
    private readonly ServerData _serverData;

    public GuiScreenAddServer(GuiMultiplayer parentScreen, ServerData serverData)
    {
        _parentScreen = parentScreen;
        _serverData = serverData;
    }

    public override void UpdateScreen()
    {
        _serverName.UpdateCursorCounter();
        _serverAddress.UpdateCursorCounter();
    }

    public override void InitGui()
    {
        Keyboard.enableRepeatEvents(true);
        Children.Clear();
        Children.Add(new Button(0, Width / 2 - 100, Height / 4 + 96 + 12, "Done"));
        Children.Add(new Button(1, Width / 2 - 100, Height / 4 + 120 + 12, "Cancel"));

        _serverName = new TextField(this, FontRenderer, Width / 2 - 100, 66, 200, 20, _serverData.Name)
        {
            Focused = true
        };
        _serverName.SetMaxStringLength(32);

        _serverAddress = new TextField(this, FontRenderer, Width / 2 - 100, 106, 200, 20, _serverData.Ip);
        _serverAddress.SetMaxStringLength(128);

        Children[0].Enabled = _serverName.Text.Length > 0 && _serverAddress.Text.Length > 0 && _serverAddress.Text.Split(":").Length > 0;
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    protected override void ActionPerformed(Button button)
    {
        if (button.Enabled)
        {
            if (button.Id == 1)
            {
                _parentScreen.ConfirmClicked(false, 0);
            }
            else if (button.Id == 0)
            {
                _serverData.Name = _serverName.Text;
                _serverData.Ip = _serverAddress.Text;
                _parentScreen.ConfirmClicked(true, 0);
            }
        }
    }

    protected override void OnKeyInput(KeyboardEventArgs e)
    {
        _serverName.TextboxKeyTyped(eventChar, eventKey);
        _serverAddress.TextboxKeyTyped(eventChar, eventKey);

        if (eventKey == Keyboard.KEY_TAB)
        {
            if (_serverName.Focused)
            {
                _serverName.Focused = false;
                _serverAddress.Focused = true;
            }
            else
            {
                _serverName.Focused = true;
                _serverAddress.Focused = false;
            }
        }

        if (eventKey == Keyboard.KEY_RETURN)
        {
            ActionPerformed(Children[0]);
        }

        Children[0].Enabled = _serverName.Text.Length > 0 && _serverAddress.Text.Length > 0 && _serverAddress.Text.Split(":").Length > 0;
    }

    protected override void OnClicked(MouseEventArgs e)
    {
        base.Clicked(x, y, button);
        _serverName.Clicked(x, y, button);
        _serverAddress.Clicked(x, y, button);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();
        Gui.DrawCenteredString(FontRenderer, "Edit Server Info", Width / 2, 17, 0xFFFFFF);
        Gui.DrawString(FontRenderer, "Server Name", Width / 2 - 100, 53, 0xA0A0A0);
        Gui.DrawString(FontRenderer, "Server Address", Width / 2 - 100, 94, 0xA0A0A0);
        _serverName.DrawTextBox();
        _serverAddress.DrawTextBox();
    }
}
