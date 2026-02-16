using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using java.awt;
using java.awt.datatransfer;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiScreen : Gui
{

    public Minecraft mc;
    public int Width;
    public int Height;
    protected List<GuiButton> controlList = new();
    public bool AllowUserInput = false;
    public TextRenderer fontRenderer;
    public GuiParticle particlesGui;
    private GuiButton selectedButton = null;

    public virtual void Render(int var1, int var2, float var3)
    {
        for (int var4 = 0; var4 < controlList.Count; ++var4)
        {
            GuiButton var5 = controlList[var4];
            var5.DrawButton(mc, var1, var2);
        }

    }

    protected virtual void KeyTyped(char eventChar, int eventKey)
    {
        if (eventKey == 1)
        {
            mc.displayGuiScreen(null);
            mc.setIngameFocus();
        }

    }

    public static string GetClipboardString()
    {
        try
        {
            Transferable var0 = Toolkit.getDefaultToolkit().getSystemClipboard().getContents(null);
            if (var0 != null && var0.isDataFlavorSupported(DataFlavor.stringFlavor))
            {
                string var1 = (string)var0.getTransferData(DataFlavor.stringFlavor);
                return var1;
            }
        }
        catch (Exception)
        {
        }

        return null;
    }

    protected virtual void MouseClicked(int var1, int var2, int var3)
    {
        if (var3 == 0)
        {
            for (int var4 = 0; var4 < controlList.Count; ++var4)
            {
                GuiButton var5 = controlList[var4];
                if (var5.MousePressed(mc, var1, var2))
                {
                    selectedButton = var5;
                    mc.sndManager.playSoundFX("random.click", 1.0F, 1.0F);
                    ActionPerformed(var5);
                }
            }
        }

    }

    protected virtual void MouseMovedOrUp(int var1, int var2, int var3)
    {
        if (selectedButton != null && var3 == 0)
        {
            selectedButton.MouseReleased(var1, var2);
            selectedButton = null;
        }

    }

    protected virtual void ActionPerformed(GuiButton var1)
    {
    }

    public void SetWorldAndResolution(Minecraft var1, int var2, int var3)
    {
        particlesGui = new GuiParticle(var1);
        mc = var1;
        fontRenderer = var1.fontRenderer;
        Width = var2;
        Height = var3;
        controlList.Clear();
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

    public void HandleMouseInput()
    {
        int var1;
        int var2;
        if (Mouse.getEventButtonState())
        {
            var1 = Mouse.getEventX() * Width / mc.displayWidth;
            var2 = Height - Mouse.getEventY() * Height / mc.displayHeight - 1;
            MouseClicked(var1, var2, Mouse.getEventButton());
        }
        else
        {
            var1 = Mouse.getEventX() * Width / mc.displayWidth;
            var2 = Height - Mouse.getEventY() * Height / mc.displayHeight - 1;
            MouseMovedOrUp(var1, var2, Mouse.getEventButton());
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
        Tessellator var2 = Tessellator.instance;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/background.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        float var3 = 32.0F;
        var2.startDrawingQuads();
        var2.setColorOpaque_I(4210752);
        var2.addVertexWithUV(0.0D, Height, 0.0D, 0.0D, (double)(Height / var3 + var1));
        var2.addVertexWithUV(Width, Height, 0.0D, (double)(Width / var3), (double)(Height / var3 + var1));
        var2.addVertexWithUV(Width, 0.0D, 0.0D, (double)(Width / var3), 0 + var1);
        var2.addVertexWithUV(0.0D, 0.0D, 0.0D, 0.0D, 0 + var1);
        var2.draw();
    }

    public virtual bool DoesGuiPauseGame()
    {
        return true;
    }

    public virtual void DeleteWorld(bool var1, int var2)
    {
    }

    public virtual void SelectNextField()
    {
    }
}