using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class CycleButton : Button
{
    private readonly CycleOption _option;
    private readonly string _textFormat;
    public GameOption Option => _option;

    public CycleButton(int x, int y, CycleOption option, string textFormat)
        : base(x, y, string.Format(textFormat, option.Value))
    {
        _option = option;
        _textFormat = textFormat;
        Size = new(150, 20);
    }

    protected override void OnClicked(MouseEventArgs e)
    {
        _option.Cycle();
        Text = string.Format(_textFormat, _option.Value);
    }
}
