using BetaSharp.Client.Input;
using BetaSharp.Client.Resource.Language;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;
using java.lang;

namespace BetaSharp.Client.Guis;

public class GuiCreateWorld : GuiScreen
{
    private const int BUTTON_CREATE = 0;
    private const int BUTTON_CANCEL = 1;

    private GuiScreen parentScreen;
    private GuiTextField textboxWorldName;
    private GuiTextField textboxSeed;
    private string folderName;
    private bool createClicked;

    public GuiCreateWorld(GuiScreen parentScreen)
    {
        this.parentScreen = parentScreen;
    }

    public override void updateScreen()
    {
        textboxWorldName.updateCursorCounter();
        textboxSeed.updateCursorCounter();
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);
        controlList.clear();
        controlList.add(new GuiButton(BUTTON_CREATE, width / 2 - 100, height / 4 + 96 + 12, translations.translateKey("selectWorld.create")));
        controlList.add(new GuiButton(BUTTON_CANCEL, width / 2 - 100, height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        textboxWorldName = new GuiTextField(this, fontRenderer, width / 2 - 100, 60, 200, 20, translations.translateKey("selectWorld.newWorld"));
        textboxWorldName.isFocused = true;
        textboxWorldName.setMaxStringLength(32);
        textboxSeed = new GuiTextField(this, fontRenderer, width / 2 - 100, 116, 200, 20, "");
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
                        catch (NumberFormatException exception)
                        {
                            worldSeed = seedInput.GetHashCode();
                        }
                    }

                    mc.playerController = new PlayerControllerSP(mc);
                    mc.startWorld(folderName, textboxWorldName.getText(), worldSeed);
                    break;
                }
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
        updateFolderName();
    }

    protected override void mouseClicked(int x, int y, int button)
    {
        base.mouseClicked(x, y, button);
        textboxWorldName.mouseClicked(x, y, button);
        textboxSeed.mouseClicked(x, y, button);
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        drawDefaultBackground();
        drawCenteredString(fontRenderer, translations.translateKey("selectWorld.create"), width / 2, height / 4 - 60 + 20, 16777215);
        drawString(fontRenderer, translations.translateKey("selectWorld.enterName"), width / 2 - 100, 47, 10526880);
        drawString(fontRenderer, translations.translateKey("selectWorld.resultFolder") + " " + folderName, width / 2 - 100, 85, 10526880);
        drawString(fontRenderer, translations.translateKey("selectWorld.enterSeed"), width / 2 - 100, 104, 10526880);
        drawString(fontRenderer, translations.translateKey("selectWorld.seedInfo"), width / 2 - 100, 140, 10526880);
        textboxWorldName.drawTextBox();
        textboxSeed.drawTextBox();
        base.render(mouseX, mouseY, partialTicks);
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