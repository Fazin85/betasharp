using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using java.awt;
using java.awt.datatransfer;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiScreen : Gui
{

    public Minecraft mc;
    public int Width;
    public int Height;
    protected List<GuiButton> _controlList = new();
    public bool AllowUserInput = false;
    public TextRenderer FontRenderer;
    public GuiParticle ParticlesGui;
    private GuiButton SelectedButton = null;

    public virtual void Render(int mouseX, int mouseY, float partialTicks)
    {
        foreach (var control in _controlList)
        {
            control.DrawButton(mc, mouseX, mouseY);
        }

    }

    protected virtual void KeyTyped(char eventChar, int eventKey)
    {
        if (eventKey == Keyboard.KEY_ESCAPE)
        {
            mc.displayGuiScreen(null);
            mc.setIngameFocus();
        }

    }

    public static string GetClipboardString()
    {
        try
        {
            unsafe
            {
                if (Display.isCreated())
                {
                    return Display.getGlfw().GetClipboardString(Display.getWindowHandle());
                }
            }
        }
        catch (Exception)
        {
        }

        return "";
    }

    public static void SetClipboardString(string text)
    {
        try
        {
            unsafe
            {
                if (Display.isCreated())
                {
                    Display.getGlfw().SetClipboardString(Display.getWindowHandle(), text);
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to set clipboard string: " + text);
        }
    }

    protected virtual void MouseClicked(int mouseX, int mouseY, int button)
    {
        if (button == 0)
        {
            foreach (var control in _controlList)
            {
                if (control.MousePressed(mc, mouseX, mouseY))
                {
                    SelectedButton = control;
                    mc.sndManager.playSoundFX("random.click", 1.0F, 1.0F);
                    ActionPerformed(control);
                }
            }
        }

    }

    protected virtual void MouseMovedOrUp(int x, int y, int button)
    {
        if (SelectedButton != null && button == 0)
        {
            SelectedButton.MouseReleased(x, y);
            SelectedButton = null;
        }

    }

    protected virtual void ActionPerformed(GuiButton var1)
    {
    }

    public void SetWorldAndResolution(Minecraft mc, int width, int height)
    {
        ParticlesGui = new GuiParticle(mc);
        this.mc = mc;
        FontRenderer = mc.fontRenderer;
        Width = width;
        Height = height;
        _controlList.Clear();
        InitGui();
    }

    public virtual void InitGui()
    {
    }

    public void HandleInput()
    {
        while (Mouse.next())
        {
            HandleMouseInput();
        }

        while (Keyboard.next())
        {
            HandleKeyboardInput();
        }

    }

    public virtual void HandleMouseInput()
    {
        int x = Mouse.getEventX() * Width / mc.displayWidth;
        int y = Height - Mouse.getEventY() * Height / mc.displayHeight - 1;
        if (Mouse.getEventButtonState())
        {
            MouseClicked(x, y, Mouse.getEventButton());
        }
        else
        {
            MouseMovedOrUp(x, y, Mouse.getEventButton());
        }
    }

    public void HandleKeyboardInput()
    {
        if (Keyboard.getEventKeyState())
        {
            if (Keyboard.getEventKey() == Keyboard.KEY_F11)
            {
                mc.toggleFullscreen();
                return;
            }

            KeyTyped(Keyboard.getEventCharacter(), Keyboard.getEventKey());
        }

    }

    public virtual void UpdateScreen()
    {
    }

    public virtual void OnGuiClosed()
    {
    }

    public void DrawDefaultBackground()
    {
        DrawWorldBackground(0);
    }

    public void DrawWorldBackground(int var1)
    {
        if (mc.world != null)
        {
            DrawGradientRect(0, 0, Width, Height, 0xC0101010, 0xD0101010);
        }
        else
        {
            DrawBackground(var1);
        }

    }

    public void DrawBackground(int var1)
    {
        GLManager.GL.Disable(EnableCap.Lighting);
        GLManager.GL.Disable(EnableCap.Fog);

        Tessellator tess = Tessellator.instance;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/background.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);

        float scale = 32.0F;
        tess.startDrawingQuads();
        tess.setColorOpaque_I(0x404040);

        tess.addVertexWithUV(0.0D, Height, 0.0D, 0.0D, (double)(Height / scale + var1));
        tess.addVertexWithUV(Width, Height, 0.0D, (double)(Width / scale), (double)(Height / scale + var1));
        tess.addVertexWithUV(Width, 0.0D, 0.0D, (double)(Width / scale), 0 + var1);
        tess.addVertexWithUV(0.0D, 0.0D, 0.0D, 0.0D, 0 + var1);
        tess.draw();
    }

    public virtual bool DoesGuiPauseGame() => true;

    public virtual void DeleteWorld(bool var1, int var2) { }

    public virtual void SelectNextField() { }
}