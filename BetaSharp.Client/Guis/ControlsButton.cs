using BetaSharp.Client.Input;
using BetaSharp.Client.Options;

namespace BetaSharp.Client.Guis;

public class ControlsButton : Button
{
    private readonly KeyBinding _keybind;
    private bool _isListening;

    public ControlsButton(int x, int y, KeyBinding keybind) : base(x, y, Keyboard.getKeyName(keybind.keyCode))
    {
        _keybind = keybind;
        Size = new(70, 20);
    }

    protected override void OnFocusChanged(FocusEventArgs e)
    {
        Text = e.Focused
            ? $"> {Keyboard.getKeyName(_keybind.keyCode)} <"
            : Keyboard.getKeyName(_keybind.keyCode);
    }

    protected override void OnKeyInput(KeyboardEventArgs e)
    {
        _keybind.keyCode = e.Key;
        Text = Keyboard.getKeyName(_keybind.keyCode);
        _isListening = false;
    }
}
