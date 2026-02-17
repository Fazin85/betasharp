using BetaSharp.Client.Input;
using BetaSharp.Util;
using BetaSharp.Server;
using BetaSharp.Server.Commands;
using java.awt;
using java.awt.datatransfer;
using System.Text;

namespace BetaSharp.Client.Guis;

public class GuiChat : GuiScreen
{

    protected string _message = "";
    private int _updateCounter = 0;
    private static readonly string _allowedChars = ChatAllowedCharacters.allowedCharacters;
    private static readonly List<string> _history = new();
    private int _historyIndex = 0;
    private List<string> _lastTabCompletions = new();
    private int _tabCompletionIndex = 0;
    private string _lastTabPrefix = "";
    private int _cursorPosition = 0;
    private int _selectionStart = -1;
    private int _selectionEnd = -1;

    public override void InitGui()
    {
        Keyboard.enableRepeatEvents(true);
        _historyIndex = _history.Count;
    }

    public override void OnGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    public override void UpdateScreen()
    {
        ++_updateCounter;
    }

    public GuiChat()
    {
    }
    public GuiChat(string prefix)
    {
        _message = prefix;
    }

    public GuiChat(string prefix, bool placeCursorAtEnd)
    {
        _message = prefix;
        _cursorPosition = _message?.Length ?? 0;
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
                case Keyboard.KEY_A: // Select all
                    _selectionStart = 0;
                    _selectionEnd = _message?.Length ?? 0;
                    _cursorPosition = _selectionEnd;
                    return;
                case Keyboard.KEY_C: // Copy
                    CopySelectionToClipboard();
                    return;
                case Keyboard.KEY_X: // Cut
                    CutSelectionToClipboard();
                    return;
                case Keyboard.KEY_V: // Paste
                    PasteClipboardAtCursor();
                    return;
            }
        }

        switch (eventKey)
        {
            // Tab key for command completion
            case Keyboard.KEY_TAB:
                {
                    HandleTabCompletion();
                    break;
                }
            // Escape key
            case Keyboard.KEY_ESCAPE:
                mc.displayGuiScreen(null);
                break;
            // Enter key
            case Keyboard.KEY_RETURN:
                {
                    string msg = _message.Trim();
                    if (msg.Length > 0)
                    {
                        // Convert '&' color codes to section (§) codes for display in chat
                        string sendMsg = ConvertAmpersandToSection(msg);
                        mc.player.sendChatMessage(sendMsg);
                        _history.Add(sendMsg);
                        if (_history.Count > 100)
                        {
                            _history.RemoveAt(0);
                        }
                    }

                    mc.displayGuiScreen(null);
                    _message = "";
                    _cursorPosition = 0;
                    break;
                }
            case Keyboard.KEY_UP:
                {
                    if (Keyboard.isKeyDown(Keyboard.KEY_LMENU) || Keyboard.isKeyDown(Keyboard.KEY_RMENU))
                    {
                        if (_historyIndex > 0)
                        {
                            --_historyIndex;
                            _message = _history[_historyIndex];
                            _cursorPosition = _message.Length;
                            _lastTabCompletions.Clear();
                            _lastTabPrefix = "";
                            _tabCompletionIndex = 0;
                        }
                    }
                    break;
                }
            case Keyboard.KEY_DOWN:
                {
                    if (Keyboard.isKeyDown(Keyboard.KEY_LMENU) || Keyboard.isKeyDown(Keyboard.KEY_RMENU))
                    {
                        if (_historyIndex < _history.Count - 1)
                        {
                            ++_historyIndex;
                            _message = _history[_historyIndex];
                            _cursorPosition = _message.Length;
                            _lastTabCompletions.Clear();
                            _lastTabPrefix = "";
                            _tabCompletionIndex = 0;
                        }
                        else if (_historyIndex == _history.Count - 1)
                        {
                            _historyIndex = _history.Count;
                            _message = "";
                            _cursorPosition = 0;
                            _lastTabCompletions.Clear();
                            _lastTabPrefix = "";
                            _tabCompletionIndex = 0;
                        }
                    }
                    break;
                }
            // Backspace
            case Keyboard.KEY_BACK:
                {
                    if (_message.Length > 0 && _cursorPosition > 0)
                    {
                        _cursorPosition--;
                        _message = _message.Substring(0, _cursorPosition) + _message.Substring(_cursorPosition + 1);
                        _lastTabCompletions.Clear();
                        _lastTabPrefix = "";
                        _tabCompletionIndex = 0;
                    }

                    break;
                }
            case Keyboard.KEY_LEFT:
                {
                    if (shiftDown)
                    {
                        if (_selectionStart == -1)
                        {
                            _selectionStart = _cursorPosition;
                        }
                        if (_cursorPosition > 0) _cursorPosition--;
                        _selectionEnd = _cursorPosition;
                    }
                    else
                    {
                        if (_cursorPosition > 0)
                        {
                            _cursorPosition--;
                        }
                        ClearSelection();
                    }
                    break;
                }
            case Keyboard.KEY_RIGHT:
                {
                    if (shiftDown)
                    {
                        if (_selectionStart == -1)
                        {
                            _selectionStart = _cursorPosition;
                        }
                        if (_cursorPosition < _message.Length) _cursorPosition++;
                        _selectionEnd = _cursorPosition;
                    }
                    else
                    {
                        if (_cursorPosition < _message.Length)
                        {
                            _cursorPosition++;
                        }
                        ClearSelection();
                    }
                    break;
                }
            case Keyboard.KEY_NONE:
                {
                    break;
                }
            // All other keys
            default:
                {
                    if (_allowedChars.Contains(eventChar) && _message.Length < 100)
                    {
                        if (HasSelection())
                        {
                            DeleteSelection();
                        }

                        _message = _message.Substring(0, _cursorPosition) + eventChar + _message.Substring(_cursorPosition);
                        _cursorPosition++;
                        ClearSelection();
                        _lastTabCompletions.Clear();  // Reset tab completions when user types
                        _lastTabPrefix = "";
                        _tabCompletionIndex = 0;
                    }

                    break;
                }
        }
    }

    private void HandleTabCompletion()
    {
        // Only handle tab completion for commands (starting with /)
        if (!_message.StartsWith("/"))
        {
            return;
        }

        // Split message into parts, keeping empty parts
        string[] allParts = _message.Split(' ');
        if (allParts.Length == 0)
        {
            return;
        }

        string commandName = allParts[0]; // e.g., "/give"
        
        // If we're only completing the command name (no space yet)
        if (allParts.Length == 1 || (allParts.Length == 2 && _message.EndsWith(" ") == false && allParts[1] == ""))
        {
            HandleCommandCompletion(commandName);
            return;
        }

        // We have arguments - handle argument completion
        HandleArgumentCompletion(commandName, allParts);
    }

    private void HandleCommandCompletion(string commandPrefix)
    {
        // Get all available commands that start with the prefix
        string prefix = commandPrefix.Substring(1).ToLower(); // Remove the "/"
        List<string> matchingCommands = CommandRegistry.GetAvailableCommands()
            .Where(cmd => cmd.ToLower().StartsWith(prefix))
            .Distinct()
            .OrderBy(cmd => cmd)
            .ToList();

        if (matchingCommands.Count == 0)
        {
            return;
        }

        // Check if this is a continuation of the previous tab completion (case-insensitive)
        bool isContinuation = (_lastTabPrefix == prefix && _lastTabCompletions.Count > 0);

        if (matchingCommands.Count == 1)
        {
            // Exactly one match - auto-complete it
            _message = "/" + matchingCommands[0];
            _cursorPosition = _message.Length;
            _lastTabCompletions = matchingCommands;
            _lastTabPrefix = prefix;
            _tabCompletionIndex = 0;
        }
        else if (isContinuation)
        {
            // User pressed Tab again with same prefix - cycle to next completion
            _tabCompletionIndex = (_tabCompletionIndex + 1) % matchingCommands.Count;
            _message = "/" + matchingCommands[_tabCompletionIndex];
            _cursorPosition = _message.Length;
            // keep lastTabPrefix as the original typed prefix so cycling continues
            // lastTabPrefix remains unchanged
        }
        else
        {
            // New Tab press - show all options and set first one
            _lastTabCompletions = matchingCommands;
            _lastTabPrefix = prefix;
            _tabCompletionIndex = 0;

            // Display available completions in chat
            string completionList = "Available commands: " + string.Join(", ", matchingCommands);
            mc?.ingameGUI?.addChatMessage(completionList);

            // Auto-complete to first option
            _message = "/" + matchingCommands[0];
            _cursorPosition = _message.Length;
        }
    }

    private void HandleArgumentCompletion(string commandName, string[] allParts)
    {
        // Determine which argument we're currently completing
        // If message ends with space, we're completing a new argument
        bool completingNewArg = _message.EndsWith(" ");
        
        // Get the current argument prefix and index
        string currentArgPrefix = "";
        int argIndex; // Index relative to command (0 = first arg after command)
        
        if (completingNewArg)
        {
            // User pressed Tab after a space - completing new argument
            argIndex = allParts.Length - 1; // Number of args already complete
            currentArgPrefix = "";
        }
        else if (allParts.Length > 1)
        {
            // User is still typing current argument
            currentArgPrefix = allParts[allParts.Length - 1];
            argIndex = allParts.Length - 2; // Index of argument being completed
        }
        else
        {
            return; // Just the command
        }

        // Get completions from provider
        MinecraftServer server = mc?.internalServer;
        List<string> matchingCompletions = [];
        
        if (server != null && argIndex >= 0)
        {
            matchingCompletions = CommandCompletionProvider.GetCompletions(commandName, argIndex, currentArgPrefix, server);
        }

        if (matchingCompletions.Count == 0)
        {
            return;
        }

        // Check if this is a continuation of the previous tab completion (case-insensitive)
        bool isContinuation = (_lastTabCompletions.Count > 0 && _lastTabPrefix == (currentArgPrefix ?? "").ToLower());

        if (matchingCompletions.Count == 1)
        {
            // Exactly one match - auto-complete it
            ReplaceCurrentArgument(allParts, matchingCompletions[0], argIndex);
            _lastTabCompletions = matchingCompletions;
            _lastTabPrefix = (currentArgPrefix ?? "").ToLower();
            _tabCompletionIndex = 0;
        }
        else if (isContinuation)
        {
            // User pressed Tab again with same prefix - cycle to next completion
            _tabCompletionIndex = (_tabCompletionIndex + 1) % matchingCompletions.Count;
            ReplaceCurrentArgument(allParts, matchingCompletions[_tabCompletionIndex], argIndex);
            // keep lastTabPrefix as the original typed prefix so cycling continues
        }
        else
        {
            // New Tab press or different prefix - show all options and set first one
            _lastTabCompletions = matchingCompletions;
            _lastTabPrefix = (currentArgPrefix ?? "").ToLower();
            _tabCompletionIndex = 0;

            // Display available completions in chat
            string completionList = "Available: " + string.Join(", ", matchingCompletions);
            mc?.ingameGUI?.addChatMessage(completionList);

            // Auto-complete to first option
            ReplaceCurrentArgument(allParts, matchingCompletions[0], argIndex);
        }
    }

    private void ReplaceCurrentArgument(string[] parts, string replacement, int argIndex)
    {
        // argIndex is relative to command (0 = first arg after command)
        // parts[0] is the command, parts[1] is first arg, etc.
        int partIndex = argIndex + 1;
        
        if (argIndex < 0 || partIndex > parts.Length)
        {
            return;
        }

        if (partIndex == parts.Length)
        {
            // Adding a new argument -> avoid creating double spaces when the last part is empty
            string joined = string.Join(" ", parts);
            if (joined.EndsWith(" "))
            {
                _message = joined + replacement;
            }
            else
            {
                _message = joined + " " + replacement;
            }
        }
        else
        {
            // Replacing existing argument
            parts[partIndex] = replacement;
            _message = string.Join(" ", parts);
        }
        
        // Move cursor to end of message
        _cursorPosition = _message.Length;
    }

    private bool HasSelection()
    {
        return _selectionStart != -1 && _selectionEnd != -1 && _selectionStart != _selectionEnd;
    }

    private (int start, int end) GetSelectionRange()
    {
        if (!HasSelection()) return (0, 0);
        int s = Math.Min(_selectionStart, _selectionEnd);
        int e = Math.Max(_selectionStart, _selectionEnd);
        return (s, e);
    }

    private string GetSelectedText()
    {
        if (!HasSelection()) return "";
        var (s, e) = GetSelectionRange();
        return _message.Substring(s, e - s);
    }

    private void DeleteSelection()
    {
        if (!HasSelection()) return;
        var (s, e) = GetSelectionRange();
        _message = _message.Substring(0, s) + _message.Substring(e);
        _cursorPosition = s;
        ClearSelection();
    }

    private void ClearSelection()
    {
        _selectionStart = -1;
        _selectionEnd = -1;
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
                int maxInsert = Math.Max(0, 100 - _message.Length);
                if (clip.Length > maxInsert) clip = clip.Substring(0, maxInsert);
                _message = _message.Substring(0, _cursorPosition) + clip + _message.Substring(_cursorPosition);
                _cursorPosition += clip.Length;
                ClearSelection();
            }
        }
        catch (Exception)
        {
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawRect(2, Height - 14, Width - 2, Height - 2, 0x80000000);
        
        // Display message with cursor at correct position
        string beforeCursor = _message.Substring(0, Math.Min(_cursorPosition, _message.Length));
        string afterCursor = _message.Substring(Math.Min(_cursorPosition, _message.Length));
        string cursor = _updateCounter / 6 % 2 == 0 ? "|" : string.Empty;

        int y = Height - 12;
        int xBase = 4;
        uint normalColor = 14737632u;

        if (HasSelection())
        {
            var (s, e) = GetSelectionRange();
            string beforeSel = _message.Substring(0, s);
            string sel = _message.Substring(s, e - s);
            string afterSel = _message.Substring(e);

            // Draw before selection
            FontRenderer.drawStringWithShadow("> " + beforeSel, xBase, y, normalColor);

            // Compute widths and draw selection background
            int beforeWidth = FontRenderer.getStringWidth("> " + beforeSel);
            int selWidth = FontRenderer.getStringWidth(sel);
            DrawRect(xBase + beforeWidth, y - 1, xBase + beforeWidth + selWidth, y + 9, 0x80FFFFFFu);

            // Draw selected text in contrasting color
            FontRenderer.drawString(sel, xBase + beforeWidth, y, 0xFF000000u);

            // Draw after selection
            FontRenderer.drawStringWithShadow(afterSel, xBase + beforeWidth + selWidth, y, normalColor);

            // Draw caret at cursor position
            int caretX = xBase + FontRenderer.getStringWidth("> " + _message.Substring(0, _cursorPosition));
            DrawRect(caretX, y - 1, caretX + 1, y + 9, 0xFF000000u);
        }
        else
        {
            // Render the input literally (do not apply color codes while typing)
            FontRenderer.drawStringWithShadow("> " + beforeCursor + cursor + afterCursor, xBase, y, normalColor);
        }
        base.Render(mouseX, mouseY, partialTicks);
    }

    protected override void MouseClicked(int x, int y, int button)
    {
        if (button == 0)
        {
            if (mc.ingameGUI._hoveredItemName != null)
            {
                if (_message.Length > 0 && !_message.EndsWith(" "))
                {
                    _message = _message + " ";
                }

                _message = _message + mc.ingameGUI._hoveredItemName;
                byte maxLen = 100;
                if (_message.Length > maxLen)
                {
                    _message = _message.Substring(0, maxLen);
                }
            }
            else
            {
                base.MouseClicked(x, y, button);
            }
        }
    }

    private string ConvertAmpersandToSection(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '&' && i + 1 < input.Length)
            {
                char c = char.ToLower(input[i + 1]);
                if ("0123456789abcdefklmnor".Contains(c))
                {
                    sb.Append((char)167);
                    sb.Append(c);
                    i++;
                    continue;
                }
            }

            sb.Append(input[i]);
        }

        return sb.ToString();
    }
}