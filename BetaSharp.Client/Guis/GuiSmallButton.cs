using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiSmallButton : GuiButton
{

    private readonly EnumOptions? _optionEnum;

    public GuiSmallButton(int id, int x, int y, string displayStr) : base(id, x, y, 150, 20, displayStr)
    {
        _optionEnum = null;
    }

    public GuiSmallButton(int id, int x, int y, int buttonWidth, int buttonHeight, string displayStr) : base(id, x, y, buttonWidth, buttonHeight, displayStr)
    {
        _optionEnum = null;
    }

    public GuiSmallButton(int id, int x, int y, EnumOptions option, string displayStr) : base(id, x, y, 150, 20, displayStr)
    {
        _optionEnum = option;
    }

    public EnumOptions? Option => _optionEnum;

    public EnumOptions returnEnumOptions()
    {
        return _optionEnum!.Value;
    }
}
