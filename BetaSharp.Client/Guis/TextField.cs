using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering;
using BetaSharp.Util;
using java.awt;
using java.awt.datatransfer;

namespace BetaSharp.Client.Guis;

public class TextField : Control
{
    private readonly TextRenderer _fontRenderer;
    public override bool Focusable => true;
    public int MaxLength { get; init; }
    private int _cursorCounter;
    private int _cursorPosition;
    private int _selectionStart = -1;
    private int _selectionEnd = -1;
    private bool HasSelection => _selectionStart != -1 && _selectionEnd != -1 && _selectionStart != _selectionEnd;

    public TextField(int x, int y, TextRenderer fontRenderer, string text) : base(x, y, 200, 20)
    {
        _fontRenderer = fontRenderer;
        Text = text;
    }

    public void UpdateCursorCounter() => _cursorCounter++;

    protected override void OnKeyInput(KeyboardEventArgs e)
    {
        if (!e.IsKeyDown || !Enabled || !Focused) return;
        Console.WriteLine("KeyInput for textbox with text '" + Text + "': " + e.KeyChar + " (code " + e.Key + ")");

        // Check for Ctrl combos first
        bool ctrlDown = Keyboard.isKeyDown(Keyboard.KEY_LCONTROL) || Keyboard.isKeyDown(Keyboard.KEY_RCONTROL);
        bool shiftDown = Keyboard.isKeyDown(Keyboard.KEY_LSHIFT) || Keyboard.isKeyDown(Keyboard.KEY_RSHIFT);

        if (ctrlDown)
        {
            switch (e.Key)
            {
                case Keyboard.KEY_A:
                    // Select all
                    _selectionStart = 0;
                    _selectionEnd = Text.Length;
                    _cursorPosition = _selectionEnd;
                    return;
                case Keyboard.KEY_C:
                    // Copy
                    CopySelectionToClipboard();
                    return;
                case Keyboard.KEY_X:
                    // Cut
                    CutSelectionToClipboard();
                    return;
                case Keyboard.KEY_V:
                    // Paste
                    PasteClipboardAtCursor();
                    return;
            }
        }

        // Handle Shift+Left/Right for selection
        if (shiftDown)
        {
            switch (e.Key)
            {
                case Keyboard.KEY_LEFT:
                    if (_selectionStart == -1)
                    {
                        _selectionStart = _cursorPosition;
                    }
                    if (_cursorPosition > 0) _cursorPosition--;
                    _selectionEnd = _cursorPosition;
                    return;
                case Keyboard.KEY_RIGHT:
                    if (_selectionStart == -1)
                    {
                        _selectionStart = _cursorPosition;
                    }
                    if (_cursorPosition < Text.Length) _cursorPosition++;
                    _selectionEnd = _cursorPosition;
                    return;
            }
        }

        // Handle regular keys
        switch (e.Key)
        {
            case Keyboard.KEY_LEFT:
                if (_cursorPosition > 0) _cursorPosition--;
                ClearSelection();
                return;
            case Keyboard.KEY_RIGHT:
                if (_cursorPosition < Text.Length) _cursorPosition++;
                ClearSelection();
                return;
            case Keyboard.KEY_HOME:
                _cursorPosition = 0;
                ClearSelection();
                return;
            case Keyboard.KEY_END:
                _cursorPosition = Text.Length;
                ClearSelection();
                return;
            case Keyboard.KEY_DELETE:
                if (HasSelection)
                {
                    DeleteSelection();
                }
                else if (_cursorPosition < Text.Length)
                {
                    Text = Text.Remove(_cursorPosition, 1);
                }
                ClearSelection();
                return;
            case Keyboard.KEY_BACK:
                HandleBackspace();
                return;
        }

        // Regular character input
        if (ChatAllowedCharacters.allowedCharacters.Contains(e.KeyChar) && (Text.Length < MaxLength || MaxLength == 0))
        {
            if (HasSelection)
            {
                DeleteSelection();
            }

            SuppressTextChanged(true);
            Text = Text.Insert(_cursorPosition, e.KeyChar.ToString());
            SuppressTextChanged(false);
            _cursorPosition++;
            ClearSelection();
        }
    }

    protected override void OnFocusChanged(FocusEventArgs e)
    {
        if (e.Focused && e.OtherControl != this)
        {
            _cursorCounter = 0;
        }
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        Gui.DrawRect(X - 1, Y - 1, X + Width + 1, Y + Height + 1, 0xFFA0A0A0);
        Gui.DrawRect(X, Y, X + Width, Y + Height, 0xFF000000);

        if (Enabled)
        {
            bool showCaret = !Focused || _cursorCounter / 6 % 2 == 0;
            int safePos = Math.Clamp(_cursorPosition, 0, Text.Length);

            if (!HasSelection)
            {
                Gui.DrawString(_fontRenderer, Text, X + 4, Y + (Height - 8) / 2, 0xE0E0E0);
            }
            else
            {
                (int start, int end) = GetSelectionRange();
                string textBeforeSelection = Text[..start];
                string selectedText = Text[start..end];
                string textAfterSelection = Text[end..];
                int textX = X + 4;
                Gui.DrawString(_fontRenderer, textBeforeSelection, X + 4, Y + (Height - 8) / 2, 0xE0E0E0);
                textX += _fontRenderer.GetStringWidth(textBeforeSelection);
                int selectionWidth = _fontRenderer.GetStringWidth(selectedText);
                Gui.DrawRect(textX - 1, Y + 5, textX + selectionWidth + 1, Y + Height - 4, 0xFFFFFFFF);
                _fontRenderer.DrawString(selectedText, textX + 1, Y + (Height - 8) / 2 + 1, 0xC7C7FF);
                _fontRenderer.DrawString(selectedText, textX, Y + (Height - 8) / 2, 0x1F1FFF);
                textX += selectionWidth;
                Gui.DrawString(_fontRenderer, textAfterSelection, textX, Y + (Height - 8) / 2, 0xE0E0E0);
            }

            if (!showCaret)
                return;

            if (_cursorPosition != Text.Length)
            {
                string textBeforeCursor = Text[..safePos];
                int caretX = X + 4 + _fontRenderer.GetStringWidth(textBeforeCursor);

                Gui.DrawRect(caretX, Y + 5, caretX + 1, Y + Height - 4, 0xFFD0D0D0);
            }
            else
            {
                int caretX;
                if (Text.Length > 0) caretX = X + 5 + _fontRenderer.GetStringWidth(Text);
                else caretX = X + 4;

                Gui.DrawString(_fontRenderer, "_", caretX, Y + (Height - 8) / 2, 0xE0E0E0);
            }
        }
        else
        {
            Gui.DrawString(_fontRenderer, Text, X + 4, Y + (Height - 8) / 2, 0x707070);
        }
    }

    protected override void OnTextChanged(TextEventArgs e)
    {
        if (ShouldSuppressTextChanged) return;
        _cursorPosition = e.Text.Length;
        _selectionStart = -1;
        _selectionEnd = -1;
    }

    protected override void OnMousePressed(MouseEventArgs e)
    {
        if (!Enabled) return;
        _cursorCounter = 0;
        int clickX = e.X - X - 4;
        int pos = 0;

        while (pos < Text.Length)
        {
            int width = _fontRenderer.GetStringWidth(Text.AsSpan()[..pos]);
            if (width > clickX)
            {
                break;
            }
            pos++;
        }
        pos--;

        if (_fontRenderer.GetStringWidth(Text) <= clickX)
        {
            pos++;
        }

        _cursorPosition = pos;
        ClearSelection();
    }

    private (int start, int end) GetSelectionRange()
    {
        if (!HasSelection) return (0, 0);
        int s = Math.Min(_selectionStart, _selectionEnd);
        int e = Math.Max(_selectionStart, _selectionEnd);
        s = Math.Max(0, Math.Min(s, Text.Length));
        e = Math.Max(0, Math.Min(e, Text.Length));
        return (s, e);
    }

    private string GetSelectedText()
    {
        if (!HasSelection) return "";
        (int start, int end) = GetSelectionRange();
        return Text[start..end];
    }

    private void HandleBackspace()
    {
        if (HasSelection)
        {
            DeleteSelection();
        }
        else if (_cursorPosition > 0)
        {
            _cursorPosition--;
            Text = Text.Remove(_cursorPosition, 1);
        }
        ClearSelection();
    }

    private void DeleteSelection()
    {
        if (!HasSelection) return;
        (int start, int end) = GetSelectionRange();
        Text = Text[..start] + Text[end..];
        _cursorPosition = start;
        ClearSelection();
    }

    private void ClearSelection()
    {
        _selectionStart = -1;
        _selectionEnd = -1;
    }

    private void CopySelectionToClipboard()
    {
        if (!HasSelection) return;
        string sel = GetSelectedText();
        Screen.SetClipboardString(sel);
    }

    private void CutSelectionToClipboard()
    {
        if (!HasSelection) return;
        CopySelectionToClipboard();
        DeleteSelection();
    }

    private void PasteClipboardAtCursor()
    {
        string clip = Screen.GetClipboardString();
        if (HasSelection) DeleteSelection();
        int maxInsert = Math.Max(0, (MaxLength > 0 ? MaxLength : 32) - Text.Length);
        if (clip.Length > maxInsert) clip = clip[..maxInsert];
        Text = Text.Insert(_cursorPosition - 1, clip);
        _cursorPosition += clip.Length;
        ClearSelection();
    }
}
