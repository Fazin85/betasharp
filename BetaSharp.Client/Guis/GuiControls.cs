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
        TranslationStorage translations = TranslationStorage.Instance;
        int leftX = getLeftColumnX();

        for (int i = 0; i < _options.KeyBindings.Length; ++i)
        {
            _controlList.Add(new GuiSmallButton(i, leftX + i % 2 * 160, Height / 6 + 24 * (i >> 1), 70, 20, _options.GetOptionDisplayString(i)));
        }

        _controlList.Add(new GuiSlider((int)EnumOptions.Sensitivity, Width / 2 + 5, Height / 6 + 130, EnumOptions.Sensitivity, _options.GetKeyBinding(EnumOptions.Sensitivity), _options.GetOptionFloatValue(EnumOptions.Sensitivity)).Size(125, 20));
        _controlList.Add(new GuiSmallButton((int)EnumOptions.InvertMouse, Width / 2 - 155, Height / 6 + 130, EnumOptions.InvertMouse, _options.GetKeyBinding(EnumOptions.InvertMouse)).Size(125, 20));

        _controlList.Add(new GuiButton(ButtonDone, Width / 2 - 100, Height / 6 + 168, translations.TranslateKey("gui.done")));
        _screenTitle = translations.TranslateKey("controls.title");
    }

    protected override void ActionPerformed(GuiButton button)
    {
        for (int i = 0; i < _options.KeyBindings.Length; ++i)
        {
            _controlList[i].DisplayString = _options.GetOptionDisplayString(i);
        }

        switch (button.Id)
        {
            case ButtonDone:
                mc.displayGuiScreen(_parentScreen);
                break;
            case int id when id == (int)EnumOptions.InvertMouse:
                _options.InvertMouse = !_options.InvertMouse;
                button.DisplayString = _options.GetKeyBinding(EnumOptions.InvertMouse);
                _options.SaveOptions();
                break;
            default:
                _selectedKey = button.Id;
                button.DisplayString = "> " + _options.GetOptionDisplayString(button.Id) + " <";
                break;
        }

    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        if (_selectedKey >= 0)
        {
            _options.SetKeyBinding(_selectedKey, eventKey);
            _controlList[_selectedKey].DisplayString = _options.GetOptionDisplayString(_selectedKey);
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
        DrawCenteredString(FontRenderer, _screenTitle, Width / 2, 20, 0xFFFFFF);
        int leftX = getLeftColumnX();

        for (int i = 0; i < _options.KeyBindings.Length; ++i)
        {
            DrawString(FontRenderer, _options.GetKeyBindingDescription(i), leftX + i % 2 * 160 + 70 + 6, Height / 6 + 24 * (i >> 1) + 7, 0xFFFFFFFF);
        }

        base.Render(mouseX, mouseY, partialTicks);
    }
}
