using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Util;

namespace BetaSharp.Client.Guis;

public class GuiEditSign : GuiScreen
{

    protected string _screenTitle = "Edit sign message:";
    private readonly BlockEntitySign _entitySign;
    private int _updateCounter;
    private int _editLine = 0;
    private static readonly string _allowedCharacters = ChatAllowedCharacters.allowedCharacters;
    
    // Selection tracking per line
    private int[] _cursorPosition = new int[4];
    private int[] _selectionStart = new int[4];
    private int[] _selectionEnd = new int[4];

    public GuiEditSign(BlockEntitySign sign)
    {
        _entitySign = sign;
    }

    private const int ButtonDone = 0;

    public override void InitGui()
    {
        _controlList.Clear();
        Keyboard.enableRepeatEvents(true);
        _controlList.Add(new GuiButton(ButtonDone, Width / 2 - 100, Height / 4 + 120, "Done"));
        
        // Initialize cursor and selection arrays
        for (int i = 0; i < 4; i++)
        {
            _cursorPosition[i] = 0;
            _selectionStart[i] = -1;
            _selectionEnd[i] = -1;
        }
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
        if (mc?.world?.isRemote ?? false)
        {
            mc.getSendQueue().addToSendQueue(new UpdateSignPacket(_entitySign.x, _entitySign.y, _entitySign.z, _entitySign.Texts));
        }

    }

    public override void UpdateScreen()
    {
        ++_updateCounter;
    }

    protected override void ActionPerformed(GuiButton button)
    {
        if (button.Enabled)
        {
            switch (button.Id)
            {
                case ButtonDone:
                    _entitySign.markDirty();
                    mc?.displayGuiScreen(null);
                    break;
            }
        }
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
        // Check for Ctrl combos first
        bool ctrlDown = Keyboard.isKeyDown(Keyboard.KEY_LCONTROL) || Keyboard.isKeyDown(Keyboard.KEY_RCONTROL);
        bool shiftDown = Keyboard.isKeyDown(Keyboard.KEY_LSHIFT) || Keyboard.isKeyDown(Keyboard.KEY_RSHIFT);

        if (ctrlDown)
        {
            switch (eventKey)
            {
                case Keyboard.KEY_A:
                    // Select all on current line
                    _selectionStart[_editLine] = 0;
                    _selectionEnd[_editLine] = _entitySign.Texts[_editLine]?.Length ?? 0;
                    _cursorPosition[_editLine] = _selectionEnd[_editLine];
                    return;
                case Keyboard.KEY_C:
                    // Copy current line
                    CopyLineToClipboard();
                    return;
                case Keyboard.KEY_X:
                    // Cut current line
                    CutLineToClipboard();
                    return;
                case Keyboard.KEY_V:
                    // Paste into current line
                    PasteClipboardIntoLine();
                    return;
            }
        }

        // Handle Shift+Left/Right for selection
        if (shiftDown)
        {
            switch (eventKey)
            {
                case Keyboard.KEY_LEFT:
                    if (_selectionStart[_editLine] == -1)
                    {
                        _selectionStart[_editLine] = _cursorPosition[_editLine];
                    }
                    if (_cursorPosition[_editLine] > 0) _cursorPosition[_editLine]--;
                    _selectionEnd[_editLine] = _cursorPosition[_editLine];
                    return;
                case Keyboard.KEY_RIGHT:
                    if (_selectionStart[_editLine] == -1)
                    {
                        _selectionStart[_editLine] = _cursorPosition[_editLine];
                    }
                    if (_cursorPosition[_editLine] < _entitySign.Texts[_editLine].Length) _cursorPosition[_editLine]++;
                    _selectionEnd[_editLine] = _cursorPosition[_editLine];
                    return;
            }
        }

        // Arrow keys for navigation
        if (eventKey == 200)  // Up
        {
            _editLine = _editLine - 1 & 3;
            return;
        }

        if (eventKey == 208)  // Down
        {
            _editLine = _editLine + 1 & 3;
            return;
        }

        if (eventKey == 203)  // Left
        {
            if (_cursorPosition[_editLine] > 0) _cursorPosition[_editLine]--;
            ClearLineSelection();
            return;
        }

        if (eventKey == 205)  // Right
        {
            if (_cursorPosition[_editLine] < _entitySign.Texts[_editLine].Length) _cursorPosition[_editLine]++;
            ClearLineSelection();
            return;
        }

        // Backspace
        if (eventKey == 14)
        {
            if (HasLineSelection())
            {
                DeleteLineSelection();
            }
            else if (_entitySign.Texts[_editLine].Length > 0 && _cursorPosition[_editLine] > 0)
            {
                _cursorPosition[_editLine]--;
                _entitySign.Texts[_editLine] = _entitySign.Texts[_editLine].Substring(0, _cursorPosition[_editLine]) + _entitySign.Texts[_editLine].Substring(_cursorPosition[_editLine] + 1);
            }
            ClearLineSelection();
            return;
        }

        // Delete key
        if (eventKey == 211)
        {
            if (HasLineSelection())
            {
                DeleteLineSelection();
            }
            else if (_cursorPosition[_editLine] < _entitySign.Texts[_editLine].Length)
            {
                _entitySign.Texts[_editLine] = _entitySign.Texts[_editLine].Substring(0, _cursorPosition[_editLine]) + _entitySign.Texts[_editLine].Substring(_cursorPosition[_editLine] + 1);
            }
            ClearLineSelection();
            return;
        }

        // Enter key switches to next line
        if (eventKey == 28)
        {
            _editLine = _editLine + 1 & 3;
            return;
        }

        // Regular character input
        if (_allowedCharacters.IndexOf(eventChar) >= 0 && _entitySign.Texts[_editLine].Length < 15)
        {
            if (HasLineSelection())
            {
                DeleteLineSelection();
            }

            _entitySign.Texts[_editLine] = _entitySign.Texts[_editLine].Substring(0, _cursorPosition[_editLine]) + eventChar + _entitySign.Texts[_editLine].Substring(_cursorPosition[_editLine]);
            _cursorPosition[_editLine]++;
            ClearLineSelection();
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        if (FontRenderer != null)
        {
            DrawCenteredString(FontRenderer, _screenTitle, Width / 2, 40, 0x00FFFFFF);
        }
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(Width / 2, 0.0F, 50.0F);
        float scale = 93.75F;
        GLManager.GL.Scale(-scale, -scale, -scale);
        GLManager.GL.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
        Block signBlock = _entitySign.getBlock();
        if (signBlock == Block.Sign)
        {
            float rotation = _entitySign.getPushedBlockData() * 360 / 16.0F;
            GLManager.GL.Rotate(rotation, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Translate(0.0F, -1.0625F, 0.0F);
        }
        else
        {
            int rotationIndex = _entitySign.getPushedBlockData();
            float angle = 0.0F;
            if (rotationIndex == 2)
            {
                angle = 180.0F;
            }

            if (rotationIndex == 4)
            {
                angle = 90.0F;
            }

            if (rotationIndex == 5)
            {
                angle = -90.0F;
            }

            GLManager.GL.Rotate(angle, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Translate(0.0F, -1.0625F, 0.0F);
        }

        if (_updateCounter / 6 % 2 == 0)
        {
            _entitySign.CurrentRow = _editLine;
        }

        BlockEntityRenderer.Instance.RenderTileEntityAt(_entitySign, -0.5D, -0.75D, -0.5D, 0.0F);
        _entitySign.CurrentRow = -1;
        GLManager.GL.PopMatrix();
        base.Render(mouseX, mouseY, partialTicks);
    }

    private bool HasLineSelection()
    {
        return _selectionStart[_editLine] != -1 && _selectionEnd[_editLine] != -1 && _selectionStart[_editLine] != _selectionEnd[_editLine];
    }

    private (int start, int end) GetLineSelectionRange()
    {
        if (!HasLineSelection()) return (0, 0);
        int s = Math.Min(_selectionStart[_editLine], _selectionEnd[_editLine]);
        int e = Math.Max(_selectionStart[_editLine], _selectionEnd[_editLine]);
        string lineText = _entitySign.Texts[_editLine] ?? "";
        s = Math.Max(0, Math.Min(s, lineText.Length));
        e = Math.Max(0, Math.Min(e, lineText.Length));
        return (s, e);
    }

    private string GetSelectedLineText()
    {
        if (!HasLineSelection()) return "";
        var (s, e) = GetLineSelectionRange();
        return _entitySign.Texts[_editLine].Substring(s, e - s);
    }

    private void DeleteLineSelection()
    {
        if (!HasLineSelection()) return;
        var (s, e) = GetLineSelectionRange();
        _entitySign.Texts[_editLine] = _entitySign.Texts[_editLine].Substring(0, s) + _entitySign.Texts[_editLine].Substring(e);
        _cursorPosition[_editLine] = s;
        ClearLineSelection();
    }

    private void ClearLineSelection()
    {
        _selectionStart[_editLine] = -1;
        _selectionEnd[_editLine] = -1;
    }

    private void CopyLineToClipboard()
    {
        if (!HasLineSelection()) return;
        try
        {
            string sel = GetSelectedLineText();
            SetClipboardString(sel);
        }
        catch (Exception)
        {
        }
    }

    private void CutLineToClipboard()
    {
        if (!HasLineSelection()) return;
        CopyLineToClipboard();
        DeleteLineSelection();
    }

    private void PasteClipboardIntoLine()
    {
        try
        {
            string clip = GetClipboardString();
            clip ??= "";
            if (HasLineSelection()) DeleteLineSelection();
            int maxInsert = Math.Max(0, 15 - _entitySign.Texts[_editLine].Length);
            if (clip.Length > maxInsert) clip = clip.Substring(0, maxInsert);
            _entitySign.Texts[_editLine] = _entitySign.Texts[_editLine].Substring(0, _cursorPosition[_editLine]) + clip + _entitySign.Texts[_editLine].Substring(_cursorPosition[_editLine]);
            _cursorPosition[_editLine] += clip.Length;
            ClearLineSelection();
        }
        catch (Exception)
        {
        }
    }
}
