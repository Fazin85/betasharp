using betareborn.Client.Input;
using betareborn.Client.Resource.Language;
using betareborn.Util;
using betareborn.Util.Maths;
using betareborn.Worlds.Storage;
using java.lang;

namespace betareborn.Client.Guis
{
    public class GuiCreateWorld : GuiScreen
    {

        private GuiScreen field_22131_a;
        private GuiTextField textboxWorldName;
        private GuiTextField textboxSeed;
        private string folderName;
        private bool createClicked;

        public GuiCreateWorld(GuiScreen var1)
        {
            field_22131_a = var1;
        }

        public override void updateScreen()
        {
            textboxWorldName.updateCursorCounter();
            textboxSeed.updateCursorCounter();
        }

        public override void initGui()
        {
            TranslationStorage var1 = TranslationStorage.getInstance();
            Keyboard.enableRepeatEvents(true);
            controlList.clear();
            controlList.add(new GuiButton(0, width / 2 - 100, height / 4 + 96 + 12, var1.translateKey("selectWorld.create")));
            controlList.add(new GuiButton(1, width / 2 - 100, height / 4 + 120 + 12, var1.translateKey("gui.cancel")));
            textboxWorldName = new GuiTextField(this, fontRenderer, width / 2 - 100, 60, 200, 20, var1.translateKey("selectWorld.newWorld"));
            textboxWorldName.isFocused = true;
            textboxWorldName.setMaxStringLength(32);
            textboxSeed = new GuiTextField(this, fontRenderer, width / 2 - 100, 116, 200, 20, "");
            func_22129_j();
        }

        private void func_22129_j()
        {
            folderName = textboxWorldName.getText().Trim();
            char[] var1 = ChatAllowedCharacters.allowedCharactersArray;
            int var2 = var1.Length;

            for (int var3 = 0; var3 < var2; ++var3)
            {
                char var4 = var1[var3];
                folderName = folderName.Replace(var4, '_');
            }

            if (MathHelper.stringNullOrLengthZero(folderName))
            {
                folderName = "World";
            }

            folderName = generateUnusedFolderName(mc.getSaveLoader(), folderName);
        }

        public static string generateUnusedFolderName(WorldStorageSource var0, string var1)
        {
            while (var0.getProperties(var1) != null)
            {
                var1 = var1 + "-";
            }

            return var1;
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
                    mc.displayGuiScreen(field_22131_a);
                }
                else if (var1.id == 0)
                {
                    if (createClicked)
                    {
                        return;
                    }

                    createClicked = true;
                    long var2 = new java.util.Random().nextLong();
                    string var4 = textboxSeed.getText();
                    if (!MathHelper.stringNullOrLengthZero(var4))
                    {
                        try
                        {
                            long var5 = Long.parseLong(var4);
                            if (var5 != 0L)
                            {
                                var2 = var5;
                            }
                        }
                        catch (NumberFormatException var7)
                        {
                            var2 = var4.GetHashCode();
                        }
                    }

                    mc.playerController = new PlayerControllerSP(mc);
                    mc.startWorld(folderName, textboxWorldName.getText(), var2);
                }

            }
        }

        protected override void keyTyped(char eventChar, int eventKey)
        {
            if (textboxWorldName.isFocused)
            {
                textboxWorldName.textboxKeyTyped(eventChar, eventKey);
            }
            else
            {
                textboxSeed.textboxKeyTyped(eventChar, eventKey);
            }

            if (eventChar == 13)
            {
                actionPerformed((GuiButton)controlList.get(0));
            }

            ((GuiButton)controlList.get(0)).enabled = textboxWorldName.getText().Length > 0;
            func_22129_j();
        }

        protected override void mouseClicked(int var1, int var2, int var3)
        {
            base.mouseClicked(var1, var2, var3);
            textboxWorldName.mouseClicked(var1, var2, var3);
            textboxSeed.mouseClicked(var1, var2, var3);
        }

        public override void render(int var1, int var2, float var3)
        {
            TranslationStorage var4 = TranslationStorage.getInstance();
            drawDefaultBackground();
            drawCenteredString(fontRenderer, var4.translateKey("selectWorld.create"), width / 2, height / 4 - 60 + 20, 16777215);
            drawString(fontRenderer, var4.translateKey("selectWorld.enterName"), width / 2 - 100, 47, 10526880);
            drawString(fontRenderer, var4.translateKey("selectWorld.resultFolder") + " " + folderName, width / 2 - 100, 85, 10526880);
            drawString(fontRenderer, var4.translateKey("selectWorld.enterSeed"), width / 2 - 100, 104, 10526880);
            drawString(fontRenderer, var4.translateKey("selectWorld.seedInfo"), width / 2 - 100, 140, 10526880);
            textboxWorldName.drawTextBox();
            textboxSeed.drawTextBox();
            base.render(var1, var2, var3);
        }

        public override void selectNextField()
        {
            if (textboxWorldName.isFocused)
            {
                textboxWorldName.setFocused(false);
                textboxSeed.setFocused(true);
            }
            else
            {
                textboxWorldName.setFocused(true);
                textboxSeed.setFocused(false);
            }

        }
    }

}