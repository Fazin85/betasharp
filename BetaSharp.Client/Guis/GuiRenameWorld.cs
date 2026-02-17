using BetaSharp.Client.Input;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Client.Guis;

public class GuiRenameWorld : GuiScreen
{
    private const int ButtonRename = 0;
    private const int ButtonCancel = 1;

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
        _controlList.Add(new GuiButton(ButtonRename, Width / 2 - 100, Height / 4 + 96 + 12, translations.translateKey("selectWorld.renameButton")));
        _controlList.Add(new GuiButton(ButtonCancel, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
        WorldStorageSource worldStorage = mc.getSaveLoader();
        WorldProperties worldProperties = worldStorage.getProperties(worldFolderName);
        string currentWorldName = worldProperties.LevelName;
        nameInputField = new GuiTextField(this, FontRenderer, Width / 2 - 100, 60, 200, 20, currentWorldName)
        {
            IsFocused = true
        };
        nameInputField.SetMaxStringLength(32);
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
                case ButtonCancel:
                    mc.displayGuiScreen(parentScreen);
                    break;
                case ButtonRename:
                    WorldStorageSource worldStorage = mc.getSaveLoader();
                    worldStorage.rename(worldFolderName, nameInputField.GetText().Trim());
                    mc.displayGuiScreen(parentScreen);
                    break;
            }
        }
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        nameInputField.textboxKeyTyped(eventChar, eventKey);
        _controlList[0].Enabled = nameInputField.GetText().Trim().Length > 0;
        if (eventChar == 13)
        {
            ActionPerformed(_controlList[0]);
        }

    }

    protected override void MouseClicked(int x, int y, int button)
    {
        base.MouseClicked(x, y, button);
        nameInputField.MouseClicked(x, y, button);
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        DrawDefaultBackground();
        DrawCenteredString(FontRenderer, translations.translateKey("selectWorld.renameTitle"), Width / 2, Height / 4 - 60 + 20, 0x00FFFFFF);
        DrawString(FontRenderer, translations.translateKey("selectWorld.enterName"), Width / 2 - 100, 47, 0xA0A0A0);
        nameInputField.DrawTextBox();
        base.Render(mouseX, mouseY, partialTicks);
    }
}
