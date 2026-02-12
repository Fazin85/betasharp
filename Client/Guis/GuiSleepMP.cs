using betareborn.Client.Input;
using betareborn.Client.Network;
using betareborn.Client.Resource.Language;
using betareborn.Network.Packets.C2SPlay;

namespace betareborn.Client.Guis
{
    public class GuiSleepMP : GuiChat
    {

        public override void initGui()
        {
            Keyboard.enableRepeatEvents(true);
            TranslationStorage var1 = TranslationStorage.getInstance();
            controlList.add(new GuiButton(1, width / 2 - 100, height - 40, var1.translateKey("multiplayer.stopSleeping")));
        }

        public override void onGuiClosed()
        {
            Keyboard.enableRepeatEvents(false);
        }

        protected override void keyTyped(char eventChar, int eventKey)
        {
            if (eventKey == 1)
            {
                func_22115_j();
            }
            else if (eventKey == 28)
            {
                string var3 = message.Trim();
                if (var3.Length > 0)
                {
                    mc.player.sendChatMessage(message.Trim());
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

        protected override void actionPerformed(GuiButton var1)
        {
            if (var1.id == 1)
            {
                func_22115_j();
            }
            else
            {
                base.actionPerformed(var1);
            }

        }

        private void func_22115_j()
        {
            if (mc.player is EntityClientPlayerMP)
            {
                ClientNetworkHandler var1 = ((EntityClientPlayerMP)mc.player).sendQueue;
                var1.addToSendQueue(new ClientCommandC2SPacket(mc.player, 3));
            }

        }
    }

}