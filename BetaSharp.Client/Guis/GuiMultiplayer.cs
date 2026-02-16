using BetaSharp.Client.Input;

namespace BetaSharp.Client.Guis;

public class GuiMultiplayer : GuiScreen
{
    private const int BUTTON_CONNECT = 0;
    private const int BUTTON_CANCEL = 1;

    private readonly GuiScreen parentScreen;
    private GuiTextField serverAddressInputField;

    public GuiMultiplayer(GuiScreen parentScreen)
    {
        this.parentScreen = parentScreen;
    }

    public override void UpdateScreen()
    {
        serverAddressInputField.updateCursorCounter();
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);
        controlList.Clear();
        controlList.Add(new GuiButton(BUTTON_CONNECT, Width / 2 - 100, Height / 4 + 96 + 12, translations.translateKey("multiplayer.connect")));
        controlList.Add(new GuiButton(BUTTON_CANCEL, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        string lastServerAddress = mc.options.lastServer.Replace("_", ":");
        controlList[0].Enabled = lastServerAddress.Length > 0;
        serverAddressInputField = new GuiTextField(this, fontRenderer, Width / 2 - 100, Height / 4 - 10 + 50 + 18, 200, 20, lastServerAddress)
        {
            isFocused = true
        };
        serverAddressInputField.setMaxStringLength(128);
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
                case BUTTON_CANCEL:
                    mc.displayGuiScreen(parentScreen);
                    break;
                case BUTTON_CONNECT:
                    {
                        string serverAddress = serverAddressInputField.getText().Trim();
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
                                    addressParts = new string[] { ipv6Address, portPart };
                                }
                                else
                                {
                                    addressParts = new string[] { ipv6Address };
                                }
                            }
                        }

                        if (addressParts.Length > 2)
                        {
                            addressParts = new string[] { serverAddress };
                        }

                        mc.displayGuiScreen(new GuiConnecting(mc, addressParts[0], addressParts.Length > 1 ? parseIntWithDefault(addressParts[1], 25565) : 25565));
                        break;
                    }
            }
        }
    }

    private int parseIntWithDefault(string value, int defaultValue)
    {
        try
        {
            return java.lang.Integer.parseInt(value.Trim());
        }
        catch (Exception exception)
        {
            return defaultValue;
        }
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        serverAddressInputField.textboxKeyTyped(eventChar, eventKey);
        if (eventChar == 13)
        {
            ActionPerformed(controlList[0]);
        }

        controlList[0].Enabled = serverAddressInputField.getText().Length > 0;
    }

    protected override void MouseClicked(int x, int y, int button)
    {
        base.MouseClicked(x, y, button);
        serverAddressInputField.mouseClicked(x, y, button);
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, translations.translateKey("multiplayer.title"), Width / 2, Height / 4 - 60 + 20, 0x00FFFFFF);
        DrawString(fontRenderer, translations.translateKey("multiplayer.info1"), Width / 2 - 140, Height / 4 - 60 + 60 + 0, 10526880);
        DrawString(fontRenderer, translations.translateKey("multiplayer.info2"), Width / 2 - 140, Height / 4 - 60 + 60 + 9, 10526880);
        DrawString(fontRenderer, translations.translateKey("multiplayer.ipinfo"), Width / 2 - 140, Height / 4 - 60 + 60 + 36, 10526880);
        serverAddressInputField.drawTextBox();
        base.Render(mouseX, mouseY, partialTicks);
    }
}
