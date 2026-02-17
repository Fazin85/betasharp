using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering;
using BetaSharp.Util;
using java.awt;
using java.awt.datatransfer;

namespace BetaSharp.Client.Guis;

public class GuiTextField : Gui
{

    private readonly TextRenderer fontRenderer;
    private readonly int xPos;
    private readonly int yPos;
    private readonly int width;
    private readonly int height;
    private string text;
    private int maxStringLength;
    private int cursorCounter;
    private int cursorPosition = 0;
    private int selectionStart = -1;
    private int selectionEnd = -1;
    public bool isFocused = false;
    public bool isEnabled = true;
    private readonly GuiScreen parentGuiScreen;

    public GuiTextField(GuiScreen parentGuiScreen, TextRenderer fontRenderer, int xPos, int yPos, int width, int height, string text)
    {
        this.parentGuiScreen = parentGuiScreen;
        this.fontRenderer = fontRenderer;
        this.xPos = xPos;
        this.yPos = yPos;
        this.width = width;
        this.height = height;
        setText(text);
    }

    public void setText(string text)
    {
        this.text = text;
        cursorPosition = text?.Length ?? 0;
        selectionStart = -1;
        selectionEnd = -1;
    }

    public string getText()
    {
        return text;
    }

    public void updateCursorCounter()
    {
        ++cursorCounter;
    }

    public void textboxKeyTyped(char eventChar, int eventKey)
    {
        if (isEnabled && isFocused)
        {
            // Check for Ctrl combos first
            bool ctrlDown = Keyboard.isKeyDown(Keyboard.KEY_LCONTROL) || Keyboard.isKeyDown(Keyboard.KEY_RCONTROL);
            bool shiftDown = Keyboard.isKeyDown(Keyboard.KEY_LSHIFT) || Keyboard.isKeyDown(Keyboard.KEY_RSHIFT);

            if (ctrlDown)
            {
                switch (eventKey)
                {
                    case Keyboard.KEY_A:
                        // Select all
                        selectionStart = 0;
                        selectionEnd = text?.Length ?? 0;
                        cursorPosition = selectionEnd;
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
                switch (eventKey)
                {
                    case Keyboard.KEY_LEFT:
                        if (selectionStart == -1)
                        {
                            selectionStart = cursorPosition;
                        }
                        if (cursorPosition > 0) cursorPosition--;
                        selectionEnd = cursorPosition;
                        return;
                    case Keyboard.KEY_RIGHT:
                        if (selectionStart == -1)
                        {
                            selectionStart = cursorPosition;
                        }
                        if (cursorPosition < text.Length) cursorPosition++;
                        selectionEnd = cursorPosition;
                        return;
                }
            }

            // Handle regular keys
            switch (eventKey)
            {
                case Keyboard.KEY_LEFT:
                    if (cursorPosition > 0) cursorPosition--;
                    ClearSelection();
                    return;
                case Keyboard.KEY_RIGHT:
                    if (cursorPosition < text.Length) cursorPosition++;
                    ClearSelection();
                    return;
                case Keyboard.KEY_HOME:
                    cursorPosition = 0;
                    ClearSelection();
                    return;
                case Keyboard.KEY_END:
                    cursorPosition = text.Length;
                    ClearSelection();
                    return;
                case Keyboard.KEY_DELETE:
                    if (HasSelection())
                    {
                        DeleteSelection();
                    }
                    else if (cursorPosition < text.Length)
                    {
                        text = text.Substring(0, cursorPosition) + text.Substring(cursorPosition + 1);
                    }
                    ClearSelection();
                    return;
            }

            // Tab key
            if (eventChar == 9 || eventKey == Keyboard.KEY_TAB)
            {
                parentGuiScreen.selectNextField();
                return;
            }

            // Backspace
            if (eventKey == Keyboard.KEY_BACK)
            {
                if (HasSelection())
                {
                    DeleteSelection();
                }
                else if (text.Length > 0 && cursorPosition > 0)
                {
                    cursorPosition--;
                    text = text.Substring(0, cursorPosition) + text.Substring(cursorPosition + 1);
                }
                ClearSelection();
                return;
            }

            // Regular character input
            if (ChatAllowedCharacters.allowedCharacters.IndexOf(eventChar) >= 0 && (text.Length < maxStringLength || maxStringLength == 0))
            {
                if (HasSelection())
                {
                    DeleteSelection();
                }

                text = text.Substring(0, cursorPosition) + eventChar + text.Substring(cursorPosition);
                cursorPosition++;
                ClearSelection();
            }
        }
    }

    public void mouseClicked(int x, int y, int button)
    {
        bool isFocused = isEnabled && x >= xPos && x < xPos + width && y >= yPos && y < yPos + height;
        setFocused(isFocused);
    }

    public void setFocused(bool isFocused)
    {
        if (isFocused && !this.isFocused)
        {
            cursorCounter = 0;
        }

        this.isFocused = isFocused;
    }

    public void drawTextBox()
    {
        drawRect(xPos - 1, yPos - 1, xPos + width + 1, yPos + height + 1, 0xFFA0A0A0);
        drawRect(xPos, yPos, xPos + width, yPos + height, 0xFF000000);
        if (isEnabled)
        {
            bool showCursor = isFocused && cursorCounter / 6 % 2 == 0;
            
            int displayStartX = xPos + 4;
            int displayY = yPos + (height - 8) / 2;
            
            if (HasSelection())
            {
                var (s, e) = GetSelectionRange();
                string beforeSel = text.Substring(0, s);
                string sel = text.Substring(s, e - s);
                string afterSel = text.Substring(e);
                
                // Draw before selection
                fontRenderer.drawString(beforeSel, displayStartX, displayY, 14737632);
                
                // Compute width of before text
                int beforeWidth = fontRenderer.getStringWidth(beforeSel);
                int selWidth = fontRenderer.getStringWidth(sel);
                
                // Draw selection background
                drawRect(displayStartX + beforeWidth, displayY - 1, displayStartX + beforeWidth + selWidth, displayY + 9, 0x80FFFFFFu);
                
                // Draw selected text in contrasting color
                fontRenderer.drawString(sel, displayStartX + beforeWidth, displayY, 0xFF000000u);
                
                // Draw after selection
                fontRenderer.drawString(afterSel, displayStartX + beforeWidth + selWidth, displayY, 14737632);
            }
            else
            {
                string beforeCursor = text.Substring(0, Math.Min(cursorPosition, text.Length));
                string afterCursor = text.Substring(Math.Min(cursorPosition, text.Length));
                string cursor = (showCursor ? "|" : "");
                
                fontRenderer.drawString(beforeCursor + cursor + afterCursor, displayStartX, displayY, 14737632);
            }
        }
        else
        {
            fontRenderer.drawString(text, xPos + 4, yPos + (height - 8) / 2, 7368816);
        }
    }

    public void setMaxStringLength(int maxStringLength)
    {
        this.maxStringLength = maxStringLength;
    }

    private bool HasSelection()
    {
        return selectionStart != -1 && selectionEnd != -1 && selectionStart != selectionEnd;
    }

    private (int start, int end) GetSelectionRange()
    {
        if (!HasSelection()) return (0, 0);
        int s = Math.Min(selectionStart, selectionEnd);
        int e = Math.Max(selectionStart, selectionEnd);
        return (s, e);
    }

    private string GetSelectedText()
    {
        if (!HasSelection()) return "";
        var (s, e) = GetSelectionRange();
        return text.Substring(s, e - s);
    }

    private void DeleteSelection()
    {
        if (!HasSelection()) return;
        var (s, e) = GetSelectionRange();
        text = text.Substring(0, s) + text.Substring(e);
        cursorPosition = s;
        ClearSelection();
    }

    private void ClearSelection()
    {
        selectionStart = -1;
        selectionEnd = -1;
    }

    private void CopySelectionToClipboard()
    {
        if (!HasSelection()) return;
        try
        {
            string sel = GetSelectedText();
            StringSelection ss = new(sel);
            Toolkit.getDefaultToolkit().getSystemClipboard().setContents(ss, null);
        }
        catch (Exception)
        {
        }
    }

    private void CutSelectionToClipboard()
    {
        if (!HasSelection()) return;
        CopySelectionToClipboard();
        DeleteSelection();
    }

    private void PasteClipboardAtCursor()
    {
        try
        {
            Transferable t = Toolkit.getDefaultToolkit().getSystemClipboard().getContents(null);
            if (t != null && t.isDataFlavorSupported(DataFlavor.stringFlavor))
            {
                string clip = (string)t.getTransferData(DataFlavor.stringFlavor);
                clip ??= "";
                if (HasSelection()) DeleteSelection();
                int maxInsert = Math.Max(0, (maxStringLength > 0 ? maxStringLength : 32) - text.Length);
                if (clip.Length > maxInsert) clip = clip.Substring(0, maxInsert);
                text = text.Substring(0, cursorPosition) + clip + text.Substring(cursorPosition);
                cursorPosition += clip.Length;
                ClearSelection();
            }
        }
        catch (Exception)
        {
        }
    }
}
