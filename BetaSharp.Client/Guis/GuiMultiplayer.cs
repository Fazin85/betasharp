using BetaSharp.Client.Input;

namespace BetaSharp.Client.Guis;

public class GuiMultiplayer : GuiScreen
{
    private const int ButtonConnect = 0;
    private const int ButtonCancel = 1;

    private readonly GuiScreen _parentScreen;
    private GuiTextField _serverAddressInputField;

    public GuiMultiplayer(GuiScreen parentScreen)
    {
        _parentScreen = parentScreen;
    }

    public override void UpdateScreen()
    {
        _serverAddressInputField.updateCursorCounter();
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);
        _controlList.Clear();
        _controlList.Add(new GuiButton(ButtonConnect, Width / 2 - 100, Height / 4 + 96 + 12, translations.translateKey("multiplayer.connect")));
        _controlList.Add(new GuiButton(ButtonCancel, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        string lastServerAddress = mc.options.lastServer.Replace("_", ":");
        _controlList[0].Enabled = lastServerAddress.Length > 0;
        _serverAddressInputField = new GuiTextField(this, FontRenderer, Width / 2 - 100, Height / 4 - 10 + 50 + 18, 200, 20, lastServerAddress)
        {
            IsFocused = true
        };
        _serverAddressInputField.SetMaxStringLength(128);
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (button.Enabled)
        {
            switch (button.Id)
            {
                case ButtonCancel:
                    mc.displayGuiScreen(_parentScreen);
                    break;
                case ButtonConnect:
                    {
                        string serverAddress = _serverAddressInputField.GetText().Trim();
                        mc.options.lastServer = serverAddress.Replace(":", "_");
                        mc.options.saveOptions();
                        string[] addressParts = serverAddress.Split(":");
                        if (serverAddress.StartsWith("["))
                        {
                            int bracketIndex = serverAddress.IndexOf("]");
                            if (bracketIndex > 0)
                            {
                                string ipv6Address = serverAddress.Substring(1, bracketIndex);
                                string portPart = serverAddress.Substring(bracketIndex + 1).Trim();
                                if (portPart.StartsWith(":") && portPart.Length > 0)
                                {
                                    portPart = portPart.Substring(1);
                                    addressParts = [ipv6Address, portPart];
                                }
                                else
                                {
                                    addressParts = [ipv6Address];
                                }
                            }
                        }

                        if (addressParts.Length > 2)
                        {
                            addressParts = [serverAddress];
                        }

                        mc.displayGuiScreen(new GuiConnecting(mc, addressParts[0], addressParts.Length > 1 ? ParseIntWithDefault(addressParts[1], 25565) : 25565));
                        break;
                    }
            }
        }
    }

    private int ParseIntWithDefault(string value, int defaultValue)
    {
        if (int.TryParse(value?.Trim(), out var result))
            return result;

        return defaultValue;
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        _serverAddressInputField.textboxKeyTyped(eventChar, eventKey);
        if (eventChar == Keyboard.KEY_RETURN)
        {
            ActionPerformed(_controlList[0]);
        }

        _controlList[0].Enabled = _serverAddressInputField.GetText().Length > 0;
    }

    protected override void MouseClicked(int x, int y, int button)
    {
        base.MouseClicked(x, y, button);
        _serverAddressInputField.MouseClicked(x, y, button);
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        DrawDefaultBackground();

        int centerX = Width / 2;
        int centerY = Height / 4;

        DrawCenteredString(FontRenderer, translations.translateKey("multiplayer.title"), centerX, centerY - 60 + 20, 0x00FFFFFF);

        DrawString(FontRenderer, translations.translateKey("multiplayer.info1"), centerX - 140, centerY - 60 + 60 + 0, 0xA0A0A0);
        DrawString(FontRenderer, translations.translateKey("multiplayer.info2"), centerX - 140, centerY - 60 + 60 + 9, 0xA0A0A0);
        DrawString(FontRenderer, translations.translateKey("multiplayer.ipinfo"), centerX - 140, centerY - 60 + 60 + 36, 0xA0A0A0);

        _serverAddressInputField.DrawTextBox();
        base.Render(mouseX, mouseY, partialTicks);
    }
}
