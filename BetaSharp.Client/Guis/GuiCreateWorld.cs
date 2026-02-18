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

    public override void updateScreen()
    {
        textboxWorldName.updateCursorCounter();
        textboxSeed.updateCursorCounter();
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);

        int centerX = width / 2;
        int centerY = height / 4;

        textboxWorldName = new GuiTextField(this, fontRenderer, centerX - 100, centerY, 200, 20, translations.translateKey("selectWorld.newWorld"))
        {
            isFocused = true
        };
        textboxWorldName.setMaxStringLength(32);
        textboxSeed = new GuiTextField(this, fontRenderer, centerX - 100, centerY + 56, 200, 20, "");

        controlList.clear();
        controlList.add(new GuiButton(BUTTON_CREATE, centerX - 100, centerY + 96 + 12, translations.translateKey("selectWorld.create")));
        controlList.add(new GuiButton(BUTTON_CANCEL, centerX - 100, centerY + 120 + 12, translations.translateKey("gui.cancel")));

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

        int centerX = width / 2;
        int centerY = height / 4;

        drawDefaultBackground();
        drawCenteredString(fontRenderer, translations.translateKey("selectWorld.create"), centerX, centerY - 60 + 20, 0x00FFFFFF);
        drawString(fontRenderer, translations.translateKey("selectWorld.enterName"), centerX - 100, centerY - 10, 0xA0A0A0);
        drawString(fontRenderer, $"{translations.translateKey("selectWorld.resultFolder")} {folderName}", centerX - 100, centerY + 24, 0xA0A0A0);
        drawString(fontRenderer, translations.translateKey("selectWorld.enterSeed"), centerX - 100, centerY + 56 - 12, 0xA0A0A0);
        drawString(fontRenderer, translations.translateKey("selectWorld.seedInfo"), centerX - 100, centerY + 56 + 24, 0xA0A0A0);
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