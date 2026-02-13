using BetaSharp.Client.Resource.Language;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Storage;
using java.text;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiSelectWorld : GuiScreen
{
    private const int BUTTON_CANCEL = 0;
    private const int BUTTON_SELECT = 1;
    private const int BUTTON_DELETE = 2;
    private const int BUTTON_CREATE = 3;
    private const int BUTTON_RENAME = 6;

    private readonly DateFormat dateFormatter = new SimpleDateFormat();
    protected GuiScreen parentScreen;
    protected string screenTitle = "Select world";
    private bool selected = false;
    private int selectedWorld;
    private List saveList;
    private GuiWorldSlot worldSlotContainer;
    private string worldNameHeader;
    private string unsupportedFormatMessage;
    private bool deleting;
    private GuiButton buttonRename;
    private GuiButton buttonSelect;
    private GuiButton buttonDelete;

    public GuiSelectWorld(GuiScreen parentScreen)
    {
        this.parentScreen = parentScreen;
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        screenTitle = translations.translateKey("selectWorld.title");
        worldNameHeader = translations.translateKey("selectWorld.world");
        unsupportedFormatMessage = "Unsupported Format!";
        loadSaves();
        worldSlotContainer = new GuiWorldSlot(this);
        worldSlotContainer.registerScrollButtons(controlList, 4, 5);
        initButtons();
    }

    private void loadSaves()
    {
        WorldStorageSource worldStorage = mc.getSaveLoader();
        saveList = worldStorage.getAll();
        Collections.sort(saveList);
        selectedWorld = -1;
    }

    protected string getSaveFileName(int worldIndex)
    {
        return ((WorldSaveInfo)saveList.get(worldIndex)).getFileName();
    }

    protected string getSaveName(int worldIndex)
    {
        string worldName = ((WorldSaveInfo)saveList.get(worldIndex)).getDisplayName();
        if (worldName == null || MathHelper.stringNullOrLengthZero(worldName))
        {
            TranslationStorage translations = TranslationStorage.getInstance();
            worldName = translations.translateKey("selectWorld.world") + " " + (worldIndex + 1);
        }

        return worldName;
    }

    public void initButtons()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        controlList.add(buttonSelect = new GuiButton(BUTTON_SELECT, width / 2 - 154, height - 52, 150, 20, translations.translateKey("selectWorld.select")));
        controlList.add(buttonRename = new GuiButton(BUTTON_RENAME, width / 2 - 154, height - 28, 70, 20, translations.translateKey("selectWorld.rename")));
        controlList.add(buttonDelete = new GuiButton(BUTTON_DELETE, width / 2 - 74, height - 28, 70, 20, translations.translateKey("selectWorld.delete")));
        controlList.add(new GuiButton(BUTTON_CREATE, width / 2 + 4, height - 52, 150, 20, translations.translateKey("selectWorld.create")));
        controlList.add(new GuiButton(BUTTON_CANCEL, width / 2 + 4, height - 28, 150, 20, translations.translateKey("gui.cancel")));
        buttonSelect.enabled = false;
        buttonRename.enabled = false;
        buttonDelete.enabled = false;
    }

    private void deleteWorld(int worldIndex)
    {
        string worldName = getSaveName(worldIndex);
        if (worldName != null)
        {
            deleting = true;
            TranslationStorage translations = TranslationStorage.getInstance();
            string deleteQuestion = translations.translateKey("selectWorld.deleteQuestion");
            string deleteWarning = "'" + worldName + "' " + translations.translateKey("selectWorld.deleteWarning");
            string deleteButtonText = translations.translateKey("selectWorld.deleteButton");
            string cancelButtonText = translations.translateKey("gui.cancel");
            GuiYesNo confirmDialog = new GuiYesNo(this, deleteQuestion, deleteWarning, deleteButtonText, cancelButtonText, worldIndex);
            mc.displayGuiScreen(confirmDialog);
        }
    }

    protected override void actionPerformed(GuiButton button)
    {
        if (button.enabled)
        {
            switch (button.id)
            {
                case BUTTON_DELETE:
                    deleteWorld(selectedWorld);
                    break;
                case BUTTON_SELECT:
                    selectWorld(selectedWorld);
                    break;
                case BUTTON_CREATE:
                    mc.displayGuiScreen(new GuiCreateWorld(this));
                    break;
                case BUTTON_RENAME:
                    mc.displayGuiScreen(new GuiRenameWorld(this, getSaveFileName(selectedWorld)));
                    break;
                case BUTTON_CANCEL:
                    mc.displayGuiScreen(parentScreen);
                    break;
                default:
                    worldSlotContainer.actionPerformed(button);
                    break;
            }
        }
    }

    public void selectWorld(int worldIndex)
    {
        if (!selected)
        {
            selected = true;
            mc.playerController = new PlayerControllerSP(mc);
            string worldFileName = getSaveFileName(worldIndex);
            if (worldFileName == null)
            {
                worldFileName = "World" + worldIndex;
            }

            mc.startWorld(worldFileName, getSaveName(worldIndex), 0L);
        }
    }

    public override void deleteWorld(bool confirmed, int worldIndex)
    {
        if (deleting)
        {
            deleting = false;
            if (confirmed)
            {
                performDelete(worldIndex);
            }

            mc.displayGuiScreen(this);
        }

    }

    private void performDelete(int worldIndex)
    {
        WorldStorageSource worldStorage = mc.getSaveLoader();
        worldStorage.flush();
        worldStorage.delete(getSaveFileName(worldIndex));
        loadSaves();
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        worldSlotContainer.drawScreen(mouseX, mouseY, partialTicks);
        drawCenteredString(fontRenderer, screenTitle, width / 2, 20, 16777215);
        base.render(mouseX, mouseY, partialTicks);
    }

    public static List getSize(GuiSelectWorld screen)
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

    public static GuiButton getSelectButton(GuiSelectWorld screen)
    {
        return screen.buttonSelect;
    }

    public static GuiButton getRenameButton(GuiSelectWorld screen)
    {
        return screen.buttonRename;
    }

    public static GuiButton getDeleteButton(GuiSelectWorld screen)
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