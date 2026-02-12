using betareborn.Client.Input;
using betareborn.Client.Resource.Language;

namespace betareborn.Client.Guis
{
    public class GuiMultiplayer : GuiScreen
    {

        private GuiScreen parentScreen;
        private GuiTextField field_22111_h;

        public GuiMultiplayer(GuiScreen var1)
        {
            parentScreen = var1;
        }

        public override void updateScreen()
        {
            field_22111_h.updateCursorCounter();
        }

        public override void initGui()
        {
            TranslationStorage var1 = TranslationStorage.getInstance();
            Keyboard.enableRepeatEvents(true);
            controlList.clear();
            controlList.add(new GuiButton(0, width / 2 - 100, height / 4 + 96 + 12, var1.translateKey("multiplayer.connect")));
            controlList.add(new GuiButton(1, width / 2 - 100, height / 4 + 120 + 12, var1.translateKey("gui.cancel")));
            string var2 = mc.options.lastServer.Replace("_", ":");
            ((GuiButton)controlList.get(0)).enabled = var2.Length > 0;
            field_22111_h = new GuiTextField(this, fontRenderer, width / 2 - 100, height / 4 - 10 + 50 + 18, 200, 20, var2);
            field_22111_h.isFocused = true;
            field_22111_h.setMaxStringLength(128);
        }

        public override void onGuiClosed()
        {
            Keyboard.enableRepeatEvents(false);
        }

        protected override void actionPerformed(GuiButton var1)
        {
            if (var1.enabled)
            {
                if (var1.id == 1)
                {
                    mc.displayGuiScreen(parentScreen);
                }
                else if (var1.id == 0)
                {
                    string var2 = field_22111_h.getText().Trim();
                    mc.options.lastServer = var2.Replace(":", "_");
                    mc.options.saveOptions();
                    string[] var3 = var2.Split(":");
                    if (var2.StartsWith("["))
                    {
                        int var4 = var2.IndexOf("]");
                        if (var4 > 0)
                        {
                            string var5 = var2.Substring(1, var4);
                            string var6 = var2.Substring(var4 + 1).Trim();
                            if (var6.StartsWith(":") && var6.Length > 0)
                            {
                                var6 = var6.Substring(1);
                                var3 = new string[] { var5, var6 };
                            }
                            else
                            {
                                var3 = new string[] { var5 };
                            }
                        }
                    }

                    if (var3.Length > 2)
                    {
                        var3 = new string[] { var2 };
                    }

                    mc.displayGuiScreen(new GuiConnecting(mc, var3[0], var3.Length > 1 ? parseIntWithDefault(var3[1], 25565) : 25565));
                }

            }
        }

        private int parseIntWithDefault(string var1, int var2)
        {
            try
            {
                return java.lang.Integer.parseInt(var1.Trim());
            }
            catch (Exception var4)
            {
                return var2;
            }
        }

        protected override void keyTyped(char eventChar, int eventKey)
        {
            field_22111_h.textboxKeyTyped(eventChar, eventKey);
            if (eventChar == 13)
            {
                actionPerformed((GuiButton)controlList.get(0));
            }

            ((GuiButton)controlList.get(0)).enabled = field_22111_h.getText().Length > 0;
        }

        protected override void mouseClicked(int var1, int var2, int var3)
        {
            base.mouseClicked(var1, var2, var3);
            field_22111_h.mouseClicked(var1, var2, var3);
        }

        public override void render(int var1, int var2, float var3)
        {
            TranslationStorage var4 = TranslationStorage.getInstance();
            drawDefaultBackground();
            drawCenteredString(fontRenderer, var4.translateKey("multiplayer.title"), width / 2, height / 4 - 60 + 20, 16777215);
            drawString(fontRenderer, var4.translateKey("multiplayer.info1"), width / 2 - 140, height / 4 - 60 + 60 + 0, 10526880);
            drawString(fontRenderer, var4.translateKey("multiplayer.info2"), width / 2 - 140, height / 4 - 60 + 60 + 9, 10526880);
            drawString(fontRenderer, var4.translateKey("multiplayer.ipinfo"), width / 2 - 140, height / 4 - 60 + 60 + 36, 10526880);
            field_22111_h.drawTextBox();
            base.render(var1, var2, var3);
        }
    }

}