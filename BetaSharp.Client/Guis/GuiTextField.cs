using BetaSharp.Client.Rendering;
using BetaSharp.Util;

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
    public bool isFocused = false;
    public bool isEnabled = true;
    private readonly GuiScreen parentGuiScreen;

    public GuiTextField(GuiScreen var1, TextRenderer var2, int var3, int var4, int var5, int var6, string var7)
    {
        parentGuiScreen = var1;
        fontRenderer = var2;
        xPos = var3;
        yPos = var4;
        width = var5;
        height = var6;
        setText(var7);
    }

    public void setText(string var1)
    {
        text = var1;
    }

    public string getText()
    {
        return text;
    }

    public void updateCursorCounter()
    {
        ++cursorCounter;
    }

    public void textboxKeyTyped(char var1, int var2)
    {
        if (isEnabled && isFocused)
        {
            if (var1 == 9)
            {
                parentGuiScreen.selectNextField();
            }

            if (var1 == 22)
            {
                string var3 = GuiScreen.getClipboardString();
                var3 ??= "";

                int var4 = 32 - text.Length;
                if (var4 > var3.Length)
                {
                    var4 = var3.Length;
                }

                if (var4 > 0)
                {
                    text = string.Concat(text, var3.AsSpan(0, var4));
                }
            }

            if (var2 == 14 && text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
            }

            if (ChatAllowedCharacters.allowedCharacters.IndexOf(var1) >= 0 && (text.Length < maxStringLength || maxStringLength == 0))
            {
                text = text + var1;
            }

        }
    }

    public void mouseClicked(int var1, int var2, int var3)
    {
        bool var4 = isEnabled && var1 >= xPos && var1 < xPos + width && var2 >= yPos && var2 < yPos + height;
        setFocused(var4);
    }

    public void setFocused(bool var1)
    {
        if (var1 && !isFocused)
        {
            cursorCounter = 0;
        }

        isFocused = var1;
    }

    public void drawTextBox()
    {
        drawRect(xPos - 1, yPos - 1, xPos + width + 1, yPos + height + 1, 0xFFA0A0A0);
        drawRect(xPos, yPos, xPos + width, yPos + height, 0xFF000000);
        if (isEnabled)
        {
            bool var1 = isFocused && cursorCounter / 6 % 2 == 0;
            drawString(fontRenderer, text + (var1 ? "_" : ""), xPos + 4, yPos + (height - 8) / 2, 14737632);
        }
        else
        {
            drawString(fontRenderer, text, xPos + 4, yPos + (height - 8) / 2, 7368816);
        }

    }

    public void setMaxStringLength(int var1)
    {
        maxStringLength = var1;
    }
}