using System;
using System.Collections.Generic;
using System.Globalization;
using BetaSharp.Client.Input;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Client.Guis;

public class GuiSelectWorld : GuiScreen
{
    private const int BUTTON_CANCEL = 0;
    private const int BUTTON_SELECT = 1;
    private const int BUTTON_DELETE = 2;
    private const int BUTTON_CREATE = 3;
    private const int BUTTON_RENAME = 6;

    // Use standard .NET formatting strings instead of Java DateFormat
    public string DateFormat { get; } = "dd/MM/yyyy HH:mm";

    protected GuiScreen parentScreen;
    protected string screenTitle = "Select world";
    private bool selected = false;

    // Properties for easier access from GuiWorldSlot
    internal int SelectedWorldIndex { get; set; } = -1;
    internal List<WorldSaveInfo> SaveList { get; private set; } = new();

    private GuiWorldSlot worldSlotContainer;
    internal string WorldNameHeader { get; private set; }
    internal string UnsupportedFormatMessage { get; private set; }

    private bool deleting;
    internal GuiButton ButtonRename { get; private set; }
    internal GuiButton ButtonSelect { get; private set; }
    internal GuiButton ButtonDelete { get; private set; }

    public GuiSelectWorld(GuiScreen parentScreen)
    {
        this.parentScreen = parentScreen;
    }

    public override void InitGui()
    {
        var translations = TranslationStorage.getInstance();
        screenTitle = translations.translateKey("selectWorld.title");
        WorldNameHeader = translations.translateKey("selectWorld.world");
        UnsupportedFormatMessage = "Unsupported Format!";

        LoadSaves();

        worldSlotContainer = new GuiWorldSlot(this);
        worldSlotContainer.RegisterScrollButtons(_controlList, 4, 5);
        InitButtons();
    }

    private void LoadSaves()
    {
        var worldStorage = mc.getSaveLoader();
        SaveList = worldStorage.GetAllSaves();

        // C# List<T>.Sort() uses the IComparable implementation on WorldSaveInfo
        SaveList.Sort();
        SelectedWorldIndex = -1;
    }

    protected string GetSaveFileName(int index) => SaveList[index].getFileName();

    protected string GetSaveName(int index)
    {
        string worldName = SaveList[index].getDisplayName();
        if (string.IsNullOrEmpty(worldName))
        {
            var translations = TranslationStorage.getInstance();
            worldName = $"{translations.translateKey("selectWorld.world")} {index + 1}";
        }
        return worldName;
    }

    public void InitButtons()
    {
        var translations = TranslationStorage.getInstance();

        _controlList.Add(ButtonSelect = new GuiButton(BUTTON_SELECT, Width / 2 - 154, Height - 52, 150, 20, translations.translateKey("selectWorld.select")));
        _controlList.Add(ButtonRename = new GuiButton(BUTTON_RENAME, Width / 2 - 154, Height - 28, 70, 20, translations.translateKey("selectWorld.rename")));
        _controlList.Add(ButtonDelete = new GuiButton(BUTTON_DELETE, Width / 2 - 74, Height - 28, 70, 20, translations.translateKey("selectWorld.delete")));
        _controlList.Add(new GuiButton(BUTTON_CREATE, Width / 2 + 4, Height - 52, 150, 20, translations.translateKey("selectWorld.create")));
        _controlList.Add(new GuiButton(BUTTON_CANCEL, Width / 2 + 4, Height - 28, 150, 20, translations.translateKey("gui.cancel")));

        ButtonSelect.Enabled = false;
        ButtonRename.Enabled = false;
        ButtonDelete.Enabled = false;
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (!button.Enabled) return;

        switch (button.Id)
        {
            case BUTTON_DELETE:
                DeleteWorldConfirmation(SelectedWorldIndex);
                break;
            case BUTTON_SELECT:
                SelectWorld(SelectedWorldIndex);
                break;
            case BUTTON_CREATE:
                mc.displayGuiScreen(new GuiCreateWorld(this));
                break;
            case BUTTON_RENAME:
                mc.displayGuiScreen(new GuiRenameWorld(this, GetSaveFileName(SelectedWorldIndex)));
                break;
            case BUTTON_CANCEL:
                mc.displayGuiScreen(parentScreen);
                break;
            default:
                worldSlotContainer.actionPerformed(button);
                break;
        }
    }

    public void SelectWorld(int worldIndex)
    {
        if (selected) return;

        selected = true;
        mc.playerController = new PlayerControllerSP(mc);

        string fileName = GetSaveFileName(worldIndex) ?? $"World{worldIndex}";
        mc.startWorld(fileName, GetSaveName(worldIndex), 0L);
    }

    private void DeleteWorldConfirmation(int worldIndex)
    {
        string worldName = GetSaveName(worldIndex);
        if (worldName != null)
        {
            deleting = true;
            var translations = TranslationStorage.getInstance();

            string question = translations.translateKey("selectWorld.deleteQuestion");
            string warning = $"'{worldName}' {translations.translateKey("selectWorld.deleteWarning")}";
            string deleteBtn = translations.translateKey("selectWorld.deleteButton");
            string cancelBtn = translations.translateKey("gui.cancel");

            mc.displayGuiScreen(new GuiYesNo(this, question, warning, deleteBtn, cancelBtn, worldIndex));
        }
    }

    public override void DeleteWorld(bool confirmed, int worldIndex)
    {
        if (deleting)
        {
            deleting = false;
            if (confirmed)
            {
                PerformDelete(worldIndex);
            }
            mc.displayGuiScreen(this);
        }
    }

    private void PerformDelete(int worldIndex)
    {
        var worldStorage = mc.getSaveLoader();
        worldStorage.Flush();
        worldStorage.DeleteWorld(GetSaveFileName(worldIndex));

        LoadSaves();
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        worldSlotContainer.drawScreen(mouseX, mouseY, partialTicks);
        DrawCenteredString(FontRenderer, screenTitle, Width / 2, 20, 0xFFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }
}
