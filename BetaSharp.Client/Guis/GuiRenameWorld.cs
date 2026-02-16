using BetaSharp.Client.Input;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Client.Guis;

public class GuiRenameWorld : GuiScreen
{
    private const int BUTTON_RENAME = 0;
    private const int BUTTON_CANCEL = 1;

    private readonly GuiScreen parentScreen;
    private GuiTextField nameInputField;
    private readonly string worldFolderName;

    public GuiRenameWorld(GuiScreen parentScreen, string worldFolderName)
    {
        this.parentScreen = parentScreen;
        this.worldFolderName = worldFolderName;
    }

    public override void updateScreen()
    {
        nameInputField.updateCursorCounter();
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);
        controlList.clear();
        controlList.add(new GuiButton(BUTTON_RENAME, width / 2 - 100, height / 4 + 96 + 12, translations.translateKey("selectWorld.renameButton")));
        controlList.add(new GuiButton(BUTTON_CANCEL, width / 2 - 100, height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        WorldStorageSource worldStorage = mc.getSaveLoader();
        WorldProperties worldProperties = worldStorage.getProperties(worldFolderName);
        string currentWorldName = worldProperties.LevelName;
        nameInputField = new GuiTextField(this, fontRenderer, width / 2 - 100, 60, 200, 20, currentWorldName)
        {
            isFocused = true
        };
        nameInputField.setMaxStringLength(32);
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
                case BUTTON_RENAME:
                    WorldStorageSource worldStorage = mc.getSaveLoader();
                    worldStorage.rename(worldFolderName, nameInputField.getText().Trim());
                    mc.displayGuiScreen(parentScreen);
                    break;
            }
        }
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
        nameInputField.textboxKeyTyped(eventChar, eventKey);
        ((GuiButton)controlList.get(0)).enabled = nameInputField.getText().Trim().Length > 0;
        if (eventChar == 13)
        {
            actionPerformed((GuiButton)controlList.get(0));
        }

    }

    protected override void mouseClicked(int x, int y, int button)
    {
        base.mouseClicked(x, y, button);
        nameInputField.mouseClicked(x, y, button);
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        drawDefaultBackground();
        drawCenteredString(fontRenderer, translations.translateKey("selectWorld.renameTitle"), width / 2, height / 4 - 60 + 20, 0x00FFFFFF);
        drawString(fontRenderer, translations.translateKey("selectWorld.enterName"), width / 2 - 100, 47, 10526880);
        nameInputField.drawTextBox();
        base.render(mouseX, mouseY, partialTicks);
    }
}
