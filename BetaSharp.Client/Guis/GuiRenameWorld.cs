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
    }

    public override void UpdateScreen()
    {
        nameInputField.UpdateCursorCounter();
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.Instance;
        Keyboard.enableRepeatEvents(true);
        Children.Clear();
        Children.Add(new GuiButton(ButtonRename, Width / 2 - 100, Height / 4 + 96 + 12, translations.TranslateKey("selectWorld.renameButton")));
        Children.Add(new GuiButton(ButtonCancel, Width / 2 - 100, Height / 4 + 120 + 12, translations.TranslateKey("gui.cancel")));
        IWorldStorageSource worldStorage = mc.getSaveLoader();
        WorldProperties? worldProperties = worldStorage.GetProperties(worldFolderName);
        string currentWorldName = worldProperties?.LevelName ?? string.Empty;
        nameInputField = new TextField(this, FontRenderer, Width / 2 - 100, 60, 200, 20, currentWorldName)
        {
            Focused = true
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

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        nameInputField.TextboxKeyTyped(eventChar, eventKey);
        Children[0].Enabled = nameInputField.Text.Trim().Length > 0;
        if (eventChar == 13)
        {
            ActionPerformed(Children[0]);
        }

    }

    protected override void MouseClicked(int x, int y, int button)
    {
        base.Clicked(x, y, button);
        nameInputField.Clicked(x, y, button);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        DrawDefaultBackground();
        Gui.DrawCenteredString(FontRenderer, translations.TranslateKey("selectWorld.renameTitle"), Width / 2, Height / 4 - 60 + 20, 0xFFFFFF);
        Gui.DrawString(FontRenderer, translations.TranslateKey("selectWorld.enterName"), Width / 2 - 100, 47, 0xA0A0A0);
        nameInputField.DrawTextBox();
        base.OnRendered(mouseX, mouseY, tickDelta);
    }
}
