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

    public override void UpdateScreen()
    {
        nameInputField.updateCursorCounter();
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        Keyboard.enableRepeatEvents(true);
        _controlList.Clear();
        _controlList.Add(new GuiButton(BUTTON_RENAME, Width / 2 - 100, Height / 4 + 96 + 12, translations.translateKey("selectWorld.renameButton")));
        _controlList.Add(new GuiButton(BUTTON_CANCEL, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        WorldStorageSource worldStorage = mc.getSaveLoader();
        WorldProperties worldProperties = worldStorage.getProperties(worldFolderName);
        string currentWorldName = worldProperties.LevelName;
        nameInputField = new GuiTextField(this, fontRenderer, Width / 2 - 100, 60, 200, 20, currentWorldName)
        {
            isFocused = true
        };
        nameInputField.setMaxStringLength(32);
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
                case BUTTON_RENAME:
                    WorldStorageSource worldStorage = mc.getSaveLoader();
                    worldStorage.rename(worldFolderName, nameInputField.getText().Trim());
                    mc.displayGuiScreen(parentScreen);
                    break;
            }
        }
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        nameInputField.textboxKeyTyped(eventChar, eventKey);
        _controlList[0].Enabled = nameInputField.getText().Trim().Length > 0;
        if (eventChar == 13)
        {
            ActionPerformed(_controlList[0]);
        }

    }

    protected override void MouseClicked(int x, int y, int button)
    {
        base.MouseClicked(x, y, button);
        nameInputField.mouseClicked(x, y, button);
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, translations.translateKey("selectWorld.renameTitle"), Width / 2, Height / 4 - 60 + 20, 0x00FFFFFF);
        DrawString(fontRenderer, translations.translateKey("selectWorld.enterName"), Width / 2 - 100, 47, 0xA0A0A0);
        nameInputField.drawTextBox();
        base.Render(mouseX, mouseY, partialTicks);
    }
}
