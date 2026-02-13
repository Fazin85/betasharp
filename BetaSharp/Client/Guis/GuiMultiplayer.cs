using BetaSharp.Client.Input;
using BetaSharp.Client.Resource.Language;

namespace BetaSharp.Client.Guis;

public class GuiMultiplayer : GuiScreen
{
    private const int BUTTON_CONNECT = 0;
    private const int BUTTON_CANCEL = 1;

    private GuiScreen parentScreen;
    private GuiTextField serverAddressInputField;

    public GuiMultiplayer(GuiScreen parentScreen)
    {
        this.parentScreen = parentScreen;
    }

    public override void updateScreen()
    {
        serverAddressInputField.updateCursorCounter();
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);
        controlList.clear();
        controlList.add(new GuiButton(BUTTON_CONNECT, width / 2 - 100, height / 4 + 96 + 12, translations.translateKey("multiplayer.connect")));
        controlList.add(new GuiButton(BUTTON_CANCEL, width / 2 - 100, height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        string lastServerAddress = mc.options.lastServer.Replace("_", ":");
        ((GuiButton)controlList.get(0)).enabled = lastServerAddress.Length > 0;
        serverAddressInputField = new GuiTextField(this, fontRenderer, width / 2 - 100, height / 4 - 10 + 50 + 18, 200, 20, lastServerAddress);
        serverAddressInputField.isFocused = true;
        serverAddressInputField.setMaxStringLength(128);
    }

    public override void onGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    protected override void actionPerformed(GuiButton button)
    {
        if (button.enabled)
        {
            switch (button.id)
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

    protected override void keyTyped(char eventChar, int eventKey)
    {
        serverAddressInputField.textboxKeyTyped(eventChar, eventKey);
        if (eventChar == 13)
        {
            actionPerformed((GuiButton)controlList.get(0));
        }

        ((GuiButton)controlList.get(0)).enabled = serverAddressInputField.getText().Length > 0;
    }

    protected override void mouseClicked(int x, int y, int button)
    {
        base.mouseClicked(x, y, button);
        serverAddressInputField.mouseClicked(x, y, button);
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        drawDefaultBackground();
        drawCenteredString(fontRenderer, translations.translateKey("multiplayer.title"), width / 2, height / 4 - 60 + 20, 16777215);
        drawString(fontRenderer, translations.translateKey("multiplayer.info1"), width / 2 - 140, height / 4 - 60 + 60 + 0, 10526880);
        drawString(fontRenderer, translations.translateKey("multiplayer.info2"), width / 2 - 140, height / 4 - 60 + 60 + 9, 10526880);
        drawString(fontRenderer, translations.translateKey("multiplayer.ipinfo"), width / 2 - 140, height / 4 - 60 + 60 + 36, 10526880);
        serverAddressInputField.drawTextBox();
        base.render(mouseX, mouseY, partialTicks);
    }
}