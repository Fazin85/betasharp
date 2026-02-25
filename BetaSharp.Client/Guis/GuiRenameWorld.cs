using BetaSharp.Client.Input;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Client.Guis;

public class GuiRenameWorld : GuiScreen
{
    private const int ButtonRename = 0;
    private const int ButtonCancel = 1;

    private readonly GuiScreen parentScreen;
    private TextField nameInputField;
    private readonly string worldFolderName;

    public GuiRenameWorld(GuiScreen parentScreen, string worldFolderName)
    {
        this.parentScreen = parentScreen;
        this.worldFolderName = worldFolderName;

        TranslationStorage translations = TranslationStorage.Instance;
        Keyboard.enableRepeatEvents(true);
        Children.Clear();
        Children.Add(new Button(ButtonRename, Width / 2 - 100, Height / 4 + 96 + 12, translations.TranslateKey("selectWorld.renameButton")));
        Children.Add(new Button(ButtonCancel, Width / 2 - 100, Height / 4 + 120 + 12, translations.TranslateKey("gui.cancel")));
        IWorldStorageSource worldStorage = mc.getSaveLoader();
        WorldProperties? worldProperties = worldStorage.GetProperties(worldFolderName);
        string currentWorldName = worldProperties?.LevelName ?? string.Empty;
        nameInputField.SetMaxStringLength(32);
        nameInputField = new TextField(this, FontRenderer, Width / 2 - 100, 60, 200, 20, currentWorldName)
        {
            Focused = true
        };
        if (e.KeyChar == Keyboard.KEY_EQUALS)
        {
            renameButton.DoClick();
        }
    }

    public override void UpdateScreen()
    {
        nameInputField.UpdateCursorCounter();
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    protected override void ActionPerformed(Button button)
    {
        if (button.Enabled)
        {
            switch (button.Id)
            {
                case ButtonCancel:
                    mc.OpenScreen(parentScreen);
                    break;
                case ButtonRename:
                    IWorldStorageSource worldStorage = mc.getSaveLoader();
                    worldStorage.Rename(worldFolderName, nameInputField.Text.Trim());
                    mc.OpenScreen(parentScreen);
                    break;
            }
        }
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        DrawDefaultBackground();
        Gui.DrawCenteredString(FontRenderer, translations.TranslateKey("selectWorld.renameTitle"), Width / 2, Height / 4 - 60 + 20, 0xFFFFFF);
        Gui.DrawString(FontRenderer, translations.TranslateKey("selectWorld.enterName"), Width / 2 - 100, 47, 0xA0A0A0);
        nameInputField.DrawTextBox();
    }
}
