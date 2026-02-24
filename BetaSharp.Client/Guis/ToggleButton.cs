using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class ToggleButton : Button
{
    private readonly BoolOption _option;
    private readonly string _textFormat;
    public GameOption Option => _option;

    public ToggleButton(int x, int y, BoolOption option, string textFormat)
        : base(x, y, string.Format(textFormat, option.Value))
    {
        _option = option;
        _textFormat = textFormat;
        Size = new(150, 20);
    }

    protected override void OnClicked(MouseEventArgs e)
    {
        _option.Toggle();
        Text = string.Format(_textFormat, _option.Value);
    }
}
