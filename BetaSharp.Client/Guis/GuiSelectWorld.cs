using BetaSharp.Client.Input;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;
using java.text;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiSelectWorld : Screen
{
    private const int BUTTON_CANCEL = 0;
    private const int BUTTON_SELECT = 1;
    private const int BUTTON_DELETE = 2;
    private const int BUTTON_CREATE = 3;
    private const int BUTTON_RENAME = 6;

    private readonly DateFormat dateFormatter = new SimpleDateFormat();
    protected Screen parentScreen;
    protected string screenTitle = "Select world";
    private bool selected;
    private int selectedWorld;
    private List<WorldSaveInfo> saveList;
    private GuiWorldList _worldListContainer;
    private string worldNameHeader;
    private string unsupportedFormatMessage;
    private bool deleting;
    private Button buttonRename;
    private Button buttonSelect;
    private Button buttonDelete;

    public GuiSelectWorld(Screen parentScreen)
    {
        this.parentScreen = parentScreen;

        TranslationStorage translations = TranslationStorage.Instance;
        Text = translations.TranslateKey("selectWorld.title");
        DisplayTitle = true;
        worldNameHeader = translations.TranslateKey("selectWorld.world");
        unsupportedFormatMessage = "Unsupported Format!";
        loadSaves();

        _worldListContainer = new GuiWorldList(this);
        buttonSelect = new(Width / 2 - 154, Height - 52, 150, 20, translations.TranslateKey("selectWorld.select")) { Enabled = false };
        buttonRename = new(Width / 2 - 154, Height - 28, 70, 20, translations.TranslateKey("selectWorld.rename")) { Enabled = false };
        buttonDelete = new(Width / 2 - 74, Height - 28, 70, 20, translations.TranslateKey("selectWorld.delete")) { Enabled = false };
        Button buttonCreate = new(Width / 2 + 4, Height - 52, 150, 20, translations.TranslateKey("selectWorld.create"));
        Button buttonCancel = new(Width / 2 + 4, Height - 28, 150, 20, translations.TranslateKey("gui.cancel"));

        buttonSelect.Clicked += (_, _) => selectWorld(selectedWorld);
        buttonRename.Clicked += (_, _) => MC.OpenScreen(new GuiRenameWorld(this, getSaveFileName(selectedWorld)));
        buttonDelete.Clicked += (_, _) => deleteWorld(selectedWorld);
        buttonCreate.Clicked += (_, _) => MC.OpenScreen(new GuiCreateWorld(this));
        buttonCancel.Clicked += (_, _) => MC.OpenScreen(parentScreen);

        AddChildren(buttonSelect, buttonRename, buttonDelete, buttonCreate, buttonCancel);
    }

    private void loadSaves()
    {
        IWorldStorageSource worldStorage = MC.getSaveLoader();
        saveList = worldStorage.GetAll();
        saveList.Sort();
        selectedWorld = -1;
    }

    protected string getSaveFileName(int worldIndex)
    {
        return saveList[worldIndex].FileName;
    }

    protected string getSaveName(int worldIndex)
    {
        string worldName = saveList[worldIndex].DisplayName;
        if (worldName == null || string.IsNullOrEmpty(worldName))
        {
            TranslationStorage translations = TranslationStorage.Instance;
            worldName = translations.TranslateKey("selectWorld.world") + " " + (worldIndex + 1);
        }

        return worldName;
    }

    private void deleteWorld(int worldIndex)
    {
        string worldName = getSaveName(worldIndex);
        if (worldName != null)
        {
            deleting = true;
            TranslationStorage translations = TranslationStorage.Instance;
            string deleteQuestion = translations.TranslateKey("selectWorld.deleteQuestion");
            string deleteWarning = "'" + worldName + "' " + translations.TranslateKey("selectWorld.deleteWarning");
            string deleteButtonText = translations.TranslateKey("selectWorld.deleteButton");
            string cancelButtonText = translations.TranslateKey("gui.cancel");
            GuiYesNo confirmDialog = new(this, deleteQuestion, deleteWarning, deleteButtonText, cancelButtonText, worldIndex);
            MC.OpenScreen(confirmDialog);
        }
    }

    public void selectWorld(int worldIndex)
    {
        if (!selected)
        {
            selected = true;
            MC.playerController = new PlayerControllerSP(MC);
            string worldFileName = getSaveFileName(worldIndex);
            worldFileName ??= "World" + worldIndex;

            MC.startWorld(worldFileName, getSaveName(worldIndex), 0L);
        }
    }

    public override void DeleteWorld(bool confirmed, int index)
    {
        if (deleting)
        {
            deleting = false;
            if (confirmed)
            {
                performDelete(index);
            }

            MC.OpenScreen(this);
        }

    }

    private void performDelete(int worldIndex)
    {
        IWorldStorageSource worldStorage = MC.getSaveLoader();
        worldStorage.Flush();
        worldStorage.Delete(getSaveFileName(worldIndex));
        loadSaves();
    }

    public static List<WorldSaveInfo> GetSize(GuiSelectWorld screen)
    {
        return screen.saveList;
    }

    public static int onElementSelected(GuiSelectWorld screen, int worldIndex)
    {
        return screen.selectedWorld = worldIndex;
    }

    public static int getSelectedWorld(GuiSelectWorld screen)
    {
        return screen.selectedWorld;
    }

    public static Button getSelectButton(GuiSelectWorld screen)
    {
        return screen.buttonSelect;
    }

    public static Button getRenameButton(GuiSelectWorld screen)
    {
        return screen.buttonRename;
    }

    public static Button getDeleteButton(GuiSelectWorld screen)
    {
        return screen.buttonDelete;
    }

    public static string getWorldNameHeader(GuiSelectWorld screen)
    {
        return screen.worldNameHeader;
    }

    public static DateFormat getDateFormatter(GuiSelectWorld screen)
    {
        return screen.dateFormatter;
    }

    public static string getUnsupportedFormatMessage(GuiSelectWorld screen)
    {
        return screen.unsupportedFormatMessage;
    }
}
