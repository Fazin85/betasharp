using BetaSharp.Client.Input;
using BetaSharp.Client.Network;
using BetaSharp.Network.Packets.C2SPlay;
using Silk.NET.GLFW;

namespace BetaSharp.Client.Guis;

public class GuiSleepMP : GuiChat
{
    private const int ButtonStopSleep = 1;

    public override void InitGui()
    {
        Keyboard.enableRepeatEvents(true);
        TranslationStorage translations = TranslationStorage.Instance;
        _controlList.Add(new GuiButton(ButtonStopSleep, Width / 2 - 100, Height - 40, translations.TranslateKey("multiplayer.stopSleeping")));
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
        base.OnGuiClosed();
    }

    protected override void KeyTyped(char eventChar, Keys eventKey)
    {
        if (eventKey == Keys.Escape)
        {
            sendStopSleepingCommand();
        }
        else if (eventKey == Keys.Enter)
        {
            string trimmed = _message.Trim();
            if (trimmed.Length > 0)
            {
                Game.player.sendChatMessage(trimmed);
            }

            _message = "";
        }
        else
        {
            base.KeyTyped(eventChar, eventKey);
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        base.Render(mouseX, mouseY, partialTicks);
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case ButtonStopSleep:
                sendStopSleepingCommand();
                break;
            default:
                base.ActionPerformed(button);
                break;
        }

    }

    private void sendStopSleepingCommand()
    {
        if (Game.player is EntityClientPlayerMP)
        {
            ClientNetworkHandler sendQueue = ((EntityClientPlayerMP)Game.player).sendQueue;
            sendQueue.addToSendQueue(ClientCommandC2SPacket.Get(Game.player, 3));
        }

    }
}
