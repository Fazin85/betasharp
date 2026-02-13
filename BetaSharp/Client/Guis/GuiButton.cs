using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiButton : Gui
{
    protected const int HOVER_STATE_DISABLED = 0;
    protected const int HOVER_STATE_NORMAL = 1;
    protected const int HOVER_STATE_HOVERED = 2;

    protected int width;
    protected int height;
    public int xPosition;
    public int yPosition;
    public string displayString;
    public int id;
    public bool enabled;
    public bool visible;

    public GuiButton(int _id, int xPos, int yPos, string displayStr) : this(_id, xPos, yPos, 200, 20, displayStr)
    {

    }

    public GuiButton(int _id, int xPos, int yPos, int wid, int hei, string displayStr)
    {
        width = 200;
        height = 20;
        enabled = true;
        visible = true;
        id = _id;
        xPosition = xPos;
        yPosition = yPos;
        width = wid;
        height = hei;
        displayString = displayStr;
    }

    protected virtual int getHoverState(bool isMouseOver)
    {
        int state = HOVER_STATE_NORMAL;
        if (!enabled)
        {
            state = HOVER_STATE_DISABLED;
        }
        else if (isMouseOver)
        {
            state = HOVER_STATE_HOVERED;
        }

        return state;
    }

    public void drawButton(Minecraft mc, int mouseX, int mouseY)
    {
        if (visible)
        {
            TextRenderer font = mc.fontRenderer;
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/gui.png"));
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            bool isHovered = mouseX >= xPosition && mouseY >= yPosition && mouseX < xPosition + width && mouseY < yPosition + height;
            int hoverState = getHoverState(isHovered);
            drawTexturedModalRect(xPosition, yPosition, 0, 46 + hoverState * 20, width / 2, height);
            drawTexturedModalRect(xPosition + width / 2, yPosition, 200 - width / 2, 46 + hoverState * 20, width / 2, height);
            mouseDragged(mc, mouseX, mouseY);
            if (!enabled)
            {
                drawCenteredString(font, displayString, xPosition + width / 2, yPosition + (height - 8) / 2, -6250336);
            }
            else if (isHovered)
            {
                drawCenteredString(font, displayString, xPosition + width / 2, yPosition + (height - 8) / 2, 16777120);
            }
            else
            {
                drawCenteredString(font, displayString, xPosition + width / 2, yPosition + (height - 8) / 2, 14737632);
            }

        }
    }

    protected virtual void mouseDragged(Minecraft mc, int mouseX, int mouseY)
    {
    }

    public virtual void mouseReleased(int mouseX, int mouseY)
    {
    }

    public virtual bool mousePressed(Minecraft mc, int mouseX, int mouseY)
    {
        return enabled && mouseX >= xPosition && mouseY >= yPosition && mouseX < xPosition + width && mouseY < yPosition + height;
    }
}