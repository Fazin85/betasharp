using betareborn.Client.Input;
using betareborn.Client.Resource.Language;
using betareborn.Worlds;
using betareborn.Worlds.Storage;

namespace betareborn.Client.Guis
{
    public class GuiRenameWorld : GuiScreen
    {

        private GuiScreen field_22112_a;
        private GuiTextField field_22114_h;
        private readonly string field_22113_i;

        public GuiRenameWorld(GuiScreen var1, string var2)
        {
            field_22112_a = var1;
            field_22113_i = var2;
        }

        public override void updateScreen()
        {
            field_22114_h.updateCursorCounter();
        }

        public override void initGui()
        {
            TranslationStorage var1 = TranslationStorage.getInstance();
            Keyboard.enableRepeatEvents(true);
            controlList.clear();
            controlList.add(new GuiButton(0, width / 2 - 100, height / 4 + 96 + 12, var1.translateKey("selectWorld.renameButton")));
            controlList.add(new GuiButton(1, width / 2 - 100, height / 4 + 120 + 12, var1.translateKey("gui.cancel")));
            WorldStorageSource var2 = mc.getSaveLoader();
            WorldProperties var3 = var2.getProperties(field_22113_i);
            string var4 = var3.getWorldName();
            field_22114_h = new GuiTextField(this, fontRenderer, width / 2 - 100, 60, 200, 20, var4);
            field_22114_h.isFocused = true;
            field_22114_h.setMaxStringLength(32);
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
                    mc.displayGuiScreen(field_22112_a);
                }
                else if (var1.id == 0)
                {
                    WorldStorageSource var2 = mc.getSaveLoader();
                    var2.rename(field_22113_i, field_22114_h.getText().Trim());
                    mc.displayGuiScreen(field_22112_a);
                }

            }
        }

        protected override void keyTyped(char eventChar, int eventKey)
        {
            field_22114_h.textboxKeyTyped(eventChar, eventKey);
            ((GuiButton)controlList.get(0)).enabled = field_22114_h.getText().Trim().Length > 0;
            if (eventChar == 13)
            {
                actionPerformed((GuiButton)controlList.get(0));
            }

        }

        protected override void mouseClicked(int var1, int var2, int var3)
        {
            base.mouseClicked(var1, var2, var3);
            field_22114_h.mouseClicked(var1, var2, var3);
        }

        public override void render(int var1, int var2, float var3)
        {
            TranslationStorage var4 = TranslationStorage.getInstance();
            drawDefaultBackground();
            drawCenteredString(fontRenderer, var4.translateKey("selectWorld.renameTitle"), width / 2, height / 4 - 60 + 20, 16777215);
            drawString(fontRenderer, var4.translateKey("selectWorld.enterName"), width / 2 - 100, 47, 10526880);
            field_22114_h.drawTextBox();
            base.render(var1, var2, var3);
        }
    }

}