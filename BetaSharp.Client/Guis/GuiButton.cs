using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;



public class GuiButton : Gui
{
    protected static readonly int HoverStateDisabled = 0;
    protected static readonly int HoverStateNormal = 1;
    protected static readonly int HoverStateHovered = 2;

    protected int _width;
    protected int _height;
    public int XPosition;
    public int YPosition;
    public string DisplayString;
    public int Id;
    public bool Enabled;
    public bool Visible;

    public GuiButton(int id, int xPos, int yPos, string displayStr) : this(id, xPos, yPos, 200, 20, displayStr)
    {

    }

    public GuiButton(int _id, int xPos, int yPos, int wid, int hei, string displayStr)
    {
        _width = 200;
        _height = 20;
        Enabled = true;
        Visible = true;
        Id = _id;
        XPosition = xPos;
        YPosition = yPos;
        _width = wid;
        _height = hei;
        DisplayString = displayStr;
    }

    protected virtual int GetHoverState(bool isMouseOver)
    {
        if (!Enabled) return HoverStateDisabled;
        if (isMouseOver) return HoverStateHovered;

        return HoverStateNormal;
    }

    public void DrawButton(Minecraft mc, int mouseX, int mouseY)
    {
        if (!Visible) return;

        TextRenderer font = mc.fontRenderer;

        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/gui.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);

        bool isHovered = mouseX >= XPosition && mouseY >= YPosition && mouseX < XPosition + _width && mouseY < YPosition + _height;
        int hoverState = GetHoverState(isHovered);

        DrawTexturedModalRect(XPosition, YPosition, 0, 46 + hoverState * 20, _width / 2, _height);
        DrawTexturedModalRect(XPosition + _width / 2, YPosition, 200 - _width / 2, 46 + hoverState * 20, _width / 2, _height);

        MouseDragged(mc, mouseX, mouseY);

        if (!Enabled)
        {
            DrawCenteredString(font, DisplayString, XPosition + _width / 2, YPosition + (_height - 8) / 2, 0xFFA0A0A0);
        }
        else if (isHovered)
        {
            DrawCenteredString(font, DisplayString, XPosition + _width / 2, YPosition + (_height - 8) / 2, 0xFFFFA0);
        }

            DrawCenteredString(font, DisplayString, XPosition + _width / 2, YPosition + (_height - 8) / 2, 0xE0E0E0);
    }

    protected virtual void MouseDragged(Minecraft mc, int mouseX, int mouseY)
    {
    }

    public virtual void MouseReleased(int mouseX, int mouseY)
    {
    }

    public virtual bool MousePressed(Minecraft mc, int mouseX, int mouseY)
    {
        return Enabled && mouseX >= XPosition && mouseY >= YPosition && mouseX < XPosition + _width && mouseY < YPosition + _height;
    }
}