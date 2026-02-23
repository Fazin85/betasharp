using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class GuiSmallButton : GuiButton
{
    private readonly GameOption? _option;

    public GuiSmallButton(int id, int x, int y, string displayStr) : base(id, x, y, 150, 20, displayStr)
    {
        _option = null;
    }

    public GuiSmallButton(int id, int x, int y, int buttonWidth, int buttonHeight, string displayStr) : base(id, x, y, buttonWidth, buttonHeight, displayStr)
    {
        _option = null;
    }

    public GuiSmallButton(int id, int x, int y, GameOption option, string displayStr) : base(id, x, y, 150, 20, displayStr)
    {
        _option = option;
    }

    public GameOption? Option => _option;

    public void ClickOption()
    {
        if (_option is BoolOption boolOpt)
        {
            boolOpt.Toggle();
        }
        else if (_option is CycleOption cycleOpt)
        {
            cycleOpt.Cycle();
        }
    }
}
