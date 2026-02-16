using BetaSharp.Client.Input;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;
using java.lang;

namespace BetaSharp.Client.Guis;

public class GuiCreateWorld : GuiScreen
{
    private const int BUTTON_CREATE = 0;
    private const int BUTTON_CANCEL = 1;

    private readonly GuiScreen parentScreen;
    private GuiTextField textboxWorldName;
    private GuiTextField textboxSeed;
    private string folderName;
    private bool createClicked;

    public GuiCreateWorld(GuiScreen parentScreen)
    {
        this.parentScreen = parentScreen;
    }

    public override void UpdateScreen()
    {
        textboxWorldName.updateCursorCounter();
        textboxSeed.updateCursorCounter();
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);
        controlList.Clear();
        controlList.Add(new GuiButton(BUTTON_CREATE, Width / 2 - 100, Height / 4 + 96 + 12, translations.translateKey("selectWorld.create")));
        controlList.Add(new GuiButton(BUTTON_CANCEL, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        textboxWorldName = new GuiTextField(this, fontRenderer, Width / 2 - 100, 60, 200, 20, translations.translateKey("selectWorld.newWorld"))
        {
            isFocused = true
        };
        textboxWorldName.setMaxStringLength(32);
        textboxSeed = new GuiTextField(this, fontRenderer, Width / 2 - 100, 116, 200, 20, "");
        updateFolderName();
    }

    private void updateFolderName()
    {
        folderName = textboxWorldName.getText().Trim();
        char[] invalidCharacters = ChatAllowedCharacters.allowedCharactersArray;
        int charCount = invalidCharacters.Length;

        for (int i = 0; i < charCount; ++i)
        {
            char invalidChar = invalidCharacters[i];
            folderName = folderName.Replace(invalidChar, '_');
        }

        if (MathHelper.stringNullOrLengthZero(folderName))
        {
            folderName = "World";
        }

        folderName = generateUnusedFolderName(mc.getSaveLoader(), folderName);
    }

    public static string generateUnusedFolderName(WorldStorageSource worldStorage, string baseFolderName)
    {
        while (worldStorage.getProperties(baseFolderName) != null)
        {
            baseFolderName = baseFolderName + "-";
        }

        return baseFolderName;
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
                case BUTTON_CREATE:
                    {
                        if (createClicked)
                        {
                            return;
                        }

                        createClicked = true;
                        long worldSeed = new java.util.Random().nextLong();
                        string seedInput = textboxSeed.getText();
                        if (!MathHelper.stringNullOrLengthZero(seedInput))
                        {
                            try
                            {
                                long parsedSeed = Long.parseLong(seedInput);
                                if (parsedSeed != 0L)
                                {
                                    worldSeed = parsedSeed;
                                }
                            }
                            catch (NumberFormatException)
                            {
                                // Java based string hashing
                                int hash = 0;
                                foreach (char c in seedInput)
                                {
                                    hash = 31 * hash + c;
                                }
                                worldSeed = hash;
                            }
                        }

                        mc.playerController = new PlayerControllerSP(mc);
                        mc.startWorld(folderName, textboxWorldName.getText(), worldSeed);
                        break;
                    }
            }
        }
    }

    protected override void KeyTyped(char eventChar, int eventKey)
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
            ActionPerformed(controlList[0]);
        }

        controlList[0].Enabled = textboxWorldName.getText().Length > 0;
        updateFolderName();
    }

    protected override void MouseClicked(int x, int y, int button)
    {
        base.MouseClicked(x, y, button);
        textboxWorldName.mouseClicked(x, y, button);
        textboxSeed.mouseClicked(x, y, button);
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, translations.translateKey("selectWorld.create"), Width / 2, Height / 4 - 60 + 20, 0x00FFFFFF);
        DrawString(fontRenderer, translations.translateKey("selectWorld.enterName"), Width / 2 - 100, 47, 10526880);
        DrawString(fontRenderer, translations.translateKey("selectWorld.resultFolder") + " " + folderName, Width / 2 - 100, 85, 10526880);
        DrawString(fontRenderer, translations.translateKey("selectWorld.enterSeed"), Width / 2 - 100, 104, 10526880);
        DrawString(fontRenderer, translations.translateKey("selectWorld.seedInfo"), Width / 2 - 100, 140, 10526880);
        textboxWorldName.drawTextBox();
        textboxSeed.drawTextBox();
        base.Render(mouseX, mouseY, partialTicks);
    }

    public override void SelectNextField()
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
