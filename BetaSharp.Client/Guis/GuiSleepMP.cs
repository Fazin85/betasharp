using BetaSharp.Client.Input;
using BetaSharp.Client.Network;
using BetaSharp.Network.Packets.C2SPlay;

namespace BetaSharp.Client.Guis;

public class GuiSleepMP : GuiChat
{
    private const int BUTTON_STOP_SLEEP = 1;

    public override void InitGui()
    {
        Keyboard.enableRepeatEvents(true);
        TranslationStorage translations = TranslationStorage.getInstance();
        _controlList.Add(new GuiButton(BUTTON_STOP_SLEEP, Width / 2 - 100, Height - 40, translations.translateKey("multiplayer.stopSleeping")));
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        if (eventKey == 1)
        {
            sendStopSleepingCommand();
        }
        else if (eventKey == 28)
        {
            string trimmed = message.Trim();
            if (trimmed.Length > 0)
            {
                mc.player.sendChatMessage(trimmed);
            }

            message = "";
        }
        else
        {
            base.KeyTyped(eventChar, eventKey);
        }

    }

    public override void Render(int var1, int var2, float var3)
    {
        base.Render(var1, var2, var3);
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case BUTTON_STOP_SLEEP:
                sendStopSleepingCommand();
                break;
            default:
                base.ActionPerformed(button);
                break;
        }

    }

    private void sendStopSleepingCommand()
    {
        if (mc.player is EntityClientPlayerMP)
        {
            ClientNetworkHandler sendQueue = ((EntityClientPlayerMP)mc.player).sendQueue;
            sendQueue.addToSendQueue(new ClientCommandC2SPacket(mc.player, 3));
        }

    }
}
