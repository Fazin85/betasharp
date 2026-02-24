using BetaSharp.Client.Input;
using BetaSharp.Client.Network;
using BetaSharp.Network.Packets.C2SPlay;

namespace BetaSharp.Client.Guis;

public class GuiSleepMP : GuiChat
{
    public override void InitGui()
    {
        Keyboard.enableRepeatEvents(true);
        TranslationStorage translations = TranslationStorage.Instance;
        Button stopSleepingButton =
            new(Width / 2 - 100, Height - 40, translations.TranslateKey("multiplayer.stopSleeping"));
        stopSleepingButton.Clicked += (_, _) => sendStopSleepingCommand();
        Children.Add(stopSleepingButton);
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    protected override void OnKeyInput(KeyboardEventArgs e)
    {
        if (e.Key == Keyboard.KEY_ESCAPE)
        {
            sendStopSleepingCommand();
        }
        else if (e.Key == Keyboard.KEY_RETURN)
        {
            string trimmed = _message.Trim();
            if (trimmed.Length > 0)
            {
                mc.player.sendChatMessage(trimmed);
            }

            _message = "";
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
