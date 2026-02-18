using BetaSharp.Client.Input;
using BetaSharp.Server;
using BetaSharp.Server.Commands;
using BetaSharp.Util;

namespace BetaSharp.Client.Guis;

public class GuiChat : GuiScreen
{

    protected string message = "";
    private int _updateCounter = 0;
    private static readonly string s_allowedChars = ChatAllowedCharacters.allowedCharacters;
    private static readonly List<string> s_history = [];
    private int _historyIndex = 0;
    private List<string> _lastTabCompletions = [];
    private int _tabCompletionIndex = 0;
    private string _lastTabPrefix = "";
    private int _cursorPosition = 0;
    private int _selectionStart = -1;
    private int _selectionEnd = -1;

    public override void initGui()
    {
        Keyboard.enableRepeatEvents(true);
        _historyIndex = s_history.Count;
    }

    public override void onGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    public override void updateScreen()
    {
        ++_updateCounter;
    }

    public override void handleMouseInput()
    {
        base.handleMouseInput();
        int dWheel = Mouse.getEventDWheel();
        if (dWheel != 0)
        {
            // Normalize scroll to 1 line per notch (usually 120 or -120)
            int scroll = dWheel > 0 ? 1 : -1;

            if (mc?.ingameGUI != null)
            {
                mc.ingameGUI.scrollChat(scroll);
            }
        }
    }

    public GuiChat()
    {
    }
    public GuiChat(string prefix)
    {
        message = prefix;
    }

    public GuiChat(string prefix, bool placeCursorAtEnd)
    {
        message = prefix;
        _cursorPosition = message?.Length ?? 0;
    }

    protected override void keyTyped(char eventChar, int eventKey)
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
                    _selectionStart = 0;
                    _selectionEnd = message?.Length ?? 0;
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
                    string msg = message.Trim();
                    if (msg.Length > 0)
                    {
                        // Special test trigger: send 64 random messages locally
                        if (msg == "!/!")
                        {
                            java.util.Random r = new();
                            for (int i = 0; i < 64; ++i)
                            {
                                int len = 8 + (int)(r.nextDouble() * 40);
                                var sb = new System.Text.StringBuilder();
                                for (int k = 0; k < len; ++k)
                                {
                                    char c = (char)('!' + (int)(r.nextDouble() * 90));
                                    sb.Append(c);
                                }
                                mc?.ingameGUI?.addChatMessage(sb.ToString());
                            }
                        }
                        else
                        {
                            // Convert '&' color codes to section (§) codes for display in chat
                            string sendMsg = ConvertAmpersandToSection(msg);
                            mc.player.sendChatMessage(sendMsg);
                            s_history.Add(sendMsg);
                            if (s_history.Count > 100)
                            {
                                s_history.RemoveAt(0);
                            }
                        }
                    }

                    mc.displayGuiScreen(null);
                    message = "";
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
                            message = s_history[_historyIndex];
                            _cursorPosition = message.Length;
                            _lastTabCompletions.Clear();
                            _lastTabPrefix = "";
                            _tabCompletionIndex = 0;
                        }
                    }
                    else
                    {
                        // Scroll up (older messages)
                        mc.ingameGUI.scrollChat(1);
                    }
                    break;
                }
            case Keyboard.KEY_DOWN:
                {
                    if (Keyboard.isKeyDown(Keyboard.KEY_LMENU) || Keyboard.isKeyDown(Keyboard.KEY_RMENU))
                    {
                        if (_historyIndex < s_history.Count - 1)
                        {
                            ++_historyIndex;
                            message = s_history[_historyIndex];
                            _cursorPosition = message.Length;
                            _lastTabCompletions.Clear();
                            _lastTabPrefix = "";
                            _tabCompletionIndex = 0;
                        }
                        else if (_historyIndex == s_history.Count - 1)
                        {
                            _historyIndex = s_history.Count;
                            message = "";
                            _cursorPosition = 0;
                            _lastTabCompletions.Clear();
                            _lastTabPrefix = "";
                            _tabCompletionIndex = 0;
                        }
                    }
                    else
                    {
                        // Scroll down (newer messages)
                        mc.ingameGUI.scrollChat(-1);
                    }
                    break;
                }
            // Backspace
            case Keyboard.KEY_BACK:
                {
                    if (message.Length > 0 && _cursorPosition > 0)
                    {
                        _cursorPosition--;
                        message = message.Substring(0, _cursorPosition) + message.Substring(_cursorPosition + 1);
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
                        if (_cursorPosition < message.Length) _cursorPosition++;
                        _selectionEnd = _cursorPosition;
                    }
                    else
                    {
                        if (_cursorPosition < message.Length)
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
                    if (s_allowedChars.Contains(eventChar) && message.Length < 100)
                    {
                        if (HasSelection())
                        {
                            DeleteSelection();
                        }

                        message = message.Substring(0, _cursorPosition) + eventChar + message.Substring(_cursorPosition);
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
        if (!message.StartsWith("/"))
        {
            return;
        }

        // Split message into parts, keeping empty parts
        string[] allParts = message.Split(' ');
        if (allParts.Length == 0)
        {
            return;
        }

        string commandName = allParts[0]; // e.g., "/give"

        // If we're only completing the command name (no space yet)
        if (allParts.Length == 1 || (allParts.Length == 2 && message.EndsWith(" ") == false && allParts[1] == ""))
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
        List<string> matchingCommands = [.. CommandRegistry.GetAvailableCommands()
            .Where(cmd => cmd.ToLower().StartsWith(prefix))
            .Distinct()
            .OrderBy(cmd => cmd)];

        if (matchingCommands.Count == 0)
        {
            return;
        }

        // Check if this is a continuation of the previous tab completion (case-insensitive)
        bool isContinuation = (_lastTabPrefix == prefix && _lastTabCompletions.Count > 0);

        if (matchingCommands.Count == 1)
        {
            // Exactly one match - auto-complete it
            message = "/" + matchingCommands[0];
            _cursorPosition = message.Length;
            _lastTabCompletions = matchingCommands;
            _lastTabPrefix = prefix;
            _tabCompletionIndex = 0;
        }
        else if (isContinuation)
        {
            // User pressed Tab again with same prefix - cycle to next completion
            _tabCompletionIndex = (_tabCompletionIndex + 1) % matchingCommands.Count;
            message = "/" + matchingCommands[_tabCompletionIndex];
            _cursorPosition = message.Length;
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
            message = "/" + matchingCommands[0];
            _cursorPosition = message.Length;
        }
    }

    private void HandleArgumentCompletion(string commandName, string[] allParts)
    {
        // Determine which argument we're currently completing
        // If message ends with space, we're completing a new argument
        bool completingNewArg = message.EndsWith(" ");

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
        MinecraftServer? server = mc?.internalServer;
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
                message = joined + replacement;
            }
            else
            {
                message = joined + " " + replacement;
            }
        }
        else
        {
            // Replacing existing argument
            parts[partIndex] = replacement;
            message = string.Join(" ", parts);
        }

        // Move cursor to end of message
        _cursorPosition = message.Length;
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
        s = Math.Max(0, Math.Min(s, message.Length));
        e = Math.Max(0, Math.Min(e, message.Length));
        return (s, e);
    }

    private string GetSelectedText()
    {
        if (!HasSelection()) return "";
        (int s, int e) = GetSelectionRange();
        return message.Substring(s, e - s);
    }

    private void DeleteSelection()
    {
        if (!HasSelection()) return;
        (int s, int e) = GetSelectionRange();
        message = message.Substring(0, s) + message.Substring(e);
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
            setClipboardString(sel);
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
            string clip = getClipboardString();
            clip ??= "";
            if (HasSelection()) DeleteSelection();
            int maxInsert = Math.Max(0, 100 - message.Length);
            if (clip.Length > maxInsert) clip = clip.Substring(0, maxInsert);
            message = message.Substring(0, _cursorPosition) + clip + message.Substring(_cursorPosition);
            _cursorPosition += clip.Length;
            ClearSelection();
        }
        catch (Exception)
        {
        }
    }

    public override void render(int var1, int var2, float var3)
    {
        drawRect(2, height - 14, width - 2, height - 2, 0x80000000);

        // Display message with cursor at correct position
        string beforeCursor = message.Substring(0, Math.Min(_cursorPosition, message.Length));
        string afterCursor = message.Substring(Math.Min(_cursorPosition, message.Length));
        string cursor = (_updateCounter / 6 % 2 == 0 ? "|" : "");

        int y = height - 12;
        int xBase = 4;
        uint normalColor = 14737632u;

        if (HasSelection())
        {
            (int s, int e) = GetSelectionRange();
            string beforeSel = message.Substring(0, s);
            string sel = message.Substring(s, e - s);
            string afterSel = message.Substring(e);

            // Draw before selection
            fontRenderer.drawStringWithShadow("> " + beforeSel, xBase, y, normalColor);

            // Compute widths and draw selection background
            int beforeWidth = fontRenderer.getStringWidth("> " + beforeSel);
            int selWidth = fontRenderer.getStringWidth(sel);
            drawRect(xBase + beforeWidth, y - 1, xBase + beforeWidth + selWidth, y + 9, 0x80FFFFFFu);

            // Draw selected text in contrasting color
            fontRenderer.drawString(sel, xBase + beforeWidth, y, 0xFF000000u);

            // Draw after selection
            fontRenderer.drawStringWithShadow(afterSel, xBase + beforeWidth + selWidth, y, normalColor);

            // Draw caret at cursor position
            int caretX = xBase + fontRenderer.getStringWidth("> " + message.Substring(0, _cursorPosition));
            drawRect(caretX, y - 1, caretX + 1, y + 9, 0xFF000000u);
        }
        else
        {
            // Render the input literally (do not apply color codes while typing)
            fontRenderer.drawStringWithShadow("> " + beforeCursor + cursor + afterCursor, xBase, y, normalColor);
        }
        base.render(var1, var2, var3);
    }

    protected override void mouseClicked(int var1, int var2, int var3)
    {
        if (var3 == 0)
        {
            // Check if clicking the small chat scrollbar
            int left = 2;
            int chatWidth = 320;
            int scrollbarX = left + chatWidth - 5;
            int scrollbarWidth = 6;
            int linesToShow = 20;
            int bottom = height - 48 + 6;
            int top = height - 48 - (linesToShow - 1) * 9;

            if (var1 >= scrollbarX && var1 <= scrollbarX + scrollbarWidth && var2 >= top && var2 <= bottom)
            {
                mc.ingameGUI?.startChatScrollbarDrag(var2, height);
                return;
            }

            if (mc.ingameGUI.field_933_a
             != null)
            {
                if (message.Length > 0 && !message.EndsWith(" "))
                {
                    message = message + " ";
                }

                message = message + mc.ingameGUI.field_933_a;
                byte var4 = 100;
                if (message.Length > var4)
                {
                    message = message.Substring(0, var4);
                }
            }
            else
            {
                base.mouseClicked(var1, var2, var3);
            }
        }
    }

    protected override void mouseMovedOrUp(int var1, int var2, int var3)
    {
        if (var3 == -1)
        {
            mc.ingameGUI?.updateChatScrollbarDrag(var2, height);
        }
        else
        {
            mc.ingameGUI?.stopChatScrollbarDrag();
            base.mouseMovedOrUp(var1, var2, var3);
        }
    }

    private string ConvertAmpersandToSection(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '&' && i + 1 < input.Length)
            {
                char c = input[i + 1];
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') ||
                    c == 'k' || c == 'K' || c == 'l' || c == 'L' || c == 'm' || c == 'M' || c == 'n' || c == 'N' || c == 'o' || c == 'O' || c == 'r' || c == 'R')
                {
                    sb.Append((char)167);
                    sb.Append(char.ToLower(c));
                    i++; // skip next char
                    continue;
                }
            }

            sb.Append(input[i]);
        }

        return sb.ToString();
    }
}
