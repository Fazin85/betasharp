using BetaSharp.Client.Input;
using BetaSharp.Client.Network;
using BetaSharp.Client.Resource.Language;
using BetaSharp.Network.Packets.C2SPlay;

namespace BetaSharp.Client.Guis;

public class GuiSleepMP : GuiChat
{
    private const int BUTTON_STOP_SLEEP = 1;

    public override void initGui()
    {
        Keyboard.enableRepeatEvents(true);
        TranslationStorage translations = TranslationStorage.getInstance();
        controlList.add(new GuiButton(BUTTON_STOP_SLEEP, width / 2 - 100, height - 40, translations.translateKey("multiplayer.stopSleeping")));
    }

    public override void onGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    protected override void keyTyped(char eventChar, int eventKey)
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
            base.keyTyped(eventChar, eventKey);
        }

    }

    public override void render(int var1, int var2, float var3)
    {
        base.render(var1, var2, var3);
    }

    protected override void actionPerformed(GuiButton button)
    {
        switch (button.id)
        {
            case BUTTON_STOP_SLEEP:
                sendStopSleepingCommand();
                break;
            default:
                base.actionPerformed(button);
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