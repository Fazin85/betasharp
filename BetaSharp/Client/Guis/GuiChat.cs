using BetaSharp.Client.Input;
using BetaSharp.Util;

namespace BetaSharp.Client.Guis;

public class GuiChat : GuiScreen
{

    protected string message = "";
    private int updateCounter = 0;
    private static readonly string allowedChars = ChatAllowedCharacters.allowedCharacters;
    private static readonly System.Collections.Generic.List<string> history = new();
    private int historyIndex = 0;

    public override void initGui()
    {
        Keyboard.enableRepeatEvents(true);
        historyIndex = history.Count;
    }

    public override void onGuiClosed()
    {
        Keyboard.enableRepeatEvents(false);
    }

    public override void updateScreen()
    {
        ++updateCounter;
    }

    public GuiChat()
    {
    }
    public GuiChat(string prefix)
    {
        message = prefix;
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
        switch (eventKey)
        {
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
                        mc.player.sendChatMessage(msg);
                        history.Add(msg);
                        if (history.Count > 100)
                        {
                            history.RemoveAt(0);
                        }
                    }

                    mc.displayGuiScreen(null);
                    break;
                }
            case Keyboard.KEY_UP:
                {
                    if (Keyboard.isKeyDown(Keyboard.KEY_LMENU) || Keyboard.isKeyDown(Keyboard.KEY_RMENU))
                    {
                        if (historyIndex > 0)
                        {
                            --historyIndex;
                            message = history[historyIndex];
                        }
                    }
                    break;
                }
            case Keyboard.KEY_DOWN:
                {
                    if (Keyboard.isKeyDown(Keyboard.KEY_LMENU) || Keyboard.isKeyDown(Keyboard.KEY_RMENU))
                    {
                        if (historyIndex < history.Count - 1)
                        {
                            ++historyIndex;
                            message = history[historyIndex];
                        }
                        else if (historyIndex == history.Count - 1)
                        {
                            historyIndex = history.Count;
                            message = "";
                        }
                    }
                    break;
                }
            // Backspace
            case Keyboard.KEY_BACK:
                {
                    if (message.Length > 0)
                    {
                        message = message.Substring(0, message.Length - 1);
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
                    if (allowedChars.Contains(eventChar) && message.Length < 100)
                    {
                        message += eventChar;
                    }

                    break;
                }
        }
    }

    public override void render(int var1, int var2, float var3)
    {
        drawRect(2, height - 14, width - 2, height - 2, 0x80000000);
        drawString(fontRenderer, "> " + message + (updateCounter / 6 % 2 == 0 ? "_" : ""), 4, height - 12,
            14737632);
        base.render(var1, var2, var3);
    }

    protected override void mouseClicked(int var1, int var2, int var3)
    {
        if (var3 == 0)
        {
            if (mc.ingameGUI.field_933_a != null)
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
}