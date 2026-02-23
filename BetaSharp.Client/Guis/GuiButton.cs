using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiButton : Control
{
    public GuiButton(string text)
    {
        Size = new(200, 20);
        Text = text;
    }

    public void DrawButton(Minecraft mc, int mouseX, int mouseY)
    {
        TextRenderer font = mc.fontRenderer;

        mc.textureManager.BindTexture(mc.textureManager.GetTextureId("/gui/gui.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);

        bool isHovered = mouseX >= X && mouseY >= Y && mouseX < X + Width && mouseY < Y + Height;
        HoverState hoverState = GetHoverState(isHovered);

        DrawTexturedModalRect(X, Y, 0, 46 + (int)hoverState * 20, Width / 2, Height);
        DrawTexturedModalRect(X + Width / 2, Y, 200 - Width / 2, 46 + (int)hoverState * 20, Width / 2, Height);

        if (_lastMouseX != mouseX || _lastMouseY != mouseY)
        {
            MouseMoved(mc, mouseX, mouseY);
        }
        RenderBackground(mc, mouseX, mouseY);

        if (!Enabled)
        {
            Gui.DrawCenteredString(font, Text, X + Width / 2, Y + (Height - 8) / 2, 0xA0A0A0);
        }
        else if (isHovered)
        {
            Gui.DrawCenteredString(font, Text, X + Width / 2, Y + (Height - 8) / 2, 0xFFFFA0);
        }
        else
        {
            Gui.DrawCenteredString(font, Text, X + Width / 2, Y + (Height - 8) / 2, 0xE0E0E0);
        }
        _lastMouseX = mouseX;
        _lastMouseY = mouseY;
    }

    protected virtual void MouseMoved(Minecraft mc, int mouseX, int mouseY)
    {
    }

    protected virtual void RenderBackground(Minecraft mc, int mouseX, int mouseY)
    {
    }

    public virtual void MouseReleased(int mouseX, int mouseY)
    {
    }
}
