using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiControls : GuiScreen
{

    private readonly GuiScreen _parentScreen;
    protected string _screenTitle = "Controls";
    private readonly GameOptions _options;
    private int _selectedKey = -1;
    private const int ButtonDone = 200;

    public GuiControls(GuiScreen parentScreen, GameOptions options)
    {
        _parentScreen = parentScreen;
        _options = options;
    }

    private int getLeftColumnX()
    {
        return Width / 2 - 155;
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        int leftX = getLeftColumnX();

        for (int i = 0; i < _options.keyBindings.Length; ++i)
        {
            _controlList.Add(new GuiSmallButton(i, leftX + i % 2 * 160, Height / 6 + 24 * (i >> 1), 70, 20, _options.getOptionDisplayString(i)));
        }

        _controlList.Add(new GuiButton(ButtonDone, Width / 2 - 100, Height / 6 + 168, translations.translateKey("gui.done")));
        _screenTitle = translations.translateKey("controls.title");
    }

    protected override void ActionPerformed(GuiButton button)
    {
        for (int i = 0; i < _options.keyBindings.Length; ++i)
        {
            _controlList[i].DisplayString = _options.getOptionDisplayString(i);
        }

        switch (button.Id)
        {
            case ButtonDone:
                mc.displayGuiScreen(_parentScreen);
                break;
            default:
                _selectedKey = button.Id;
                button.DisplayString = "> " + _options.getOptionDisplayString(button.Id) + " <";
                break;
        }

    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        if (_selectedKey >= 0)
        {
            _options.setKeyBinding(_selectedKey, eventKey);
            _controlList[_selectedKey].DisplayString = _options.getOptionDisplayString(_selectedKey);
            _selectedKey = -1;
        }
        else
        {
            base.KeyTyped(eventChar, eventKey);
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(FontRenderer, _screenTitle, Width / 2, 20, 0x00FFFFFF);
        int leftX = getLeftColumnX();

        for (int i = 0; i < _options.keyBindings.Length; ++i)
        {
            DrawString(FontRenderer, _options.getKeyBindingDescription(i), leftX + i % 2 * 160 + 70 + 6, Height / 6 + 24 * (i >> 1) + 7, 0xFFFFFFFF);
        }

        base.Render(mouseX, mouseY, partialTicks);
    }
}
