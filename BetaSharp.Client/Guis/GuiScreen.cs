using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiScreen : Gui
{

    public Minecraft mc;
    public int width;
    public int height;
    protected java.util.List controlList = new ArrayList();
    public bool field_948_f = false;
    public TextRenderer fontRenderer;
    public GuiParticle particlesGui;
    private GuiButton selectedButton = null;

    public virtual void render(int var1, int var2, float var3)
    {
        for (int var4 = 0; var4 < controlList.size(); ++var4)
        {
            GuiButton var5 = (GuiButton)controlList.get(var4);
            var5.drawButton(mc, var1, var2);
        }

    }

    protected virtual void keyTyped(char eventChar, int eventKey)
    {
        if (eventKey == 1)
        {
            mc.displayGuiScreen(null);
            mc.setIngameFocus();
        }

    }

    public static string getClipboardString()
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

    protected virtual void mouseClicked(int var1, int var2, int var3)
    {
        if (var3 == 0)
        {
            for (int var4 = 0; var4 < controlList.size(); ++var4)
            {
                GuiButton var5 = (GuiButton)controlList.get(var4);
                if (var5.mousePressed(mc, var1, var2))
                {
                    selectedButton = var5;
                    mc.sndManager.playSoundFX("random.click", 1.0F, 1.0F);
                    actionPerformed(var5);
                }
            }
        }

    }

    protected virtual void mouseMovedOrUp(int var1, int var2, int var3)
    {
        if (selectedButton != null && var3 == 0)
        {
            selectedButton.mouseReleased(var1, var2);
            selectedButton = null;
        }

    }

    protected virtual void actionPerformed(GuiButton var1)
    {
    }

    public void setWorldAndResolution(Minecraft var1, int var2, int var3)
    {
        particlesGui = new GuiParticle(var1);
        mc = var1;
        fontRenderer = var1.fontRenderer;
        width = var2;
        height = var3;
        controlList.clear();
        initGui();
    }

    public virtual void initGui()
    {
    }

    public void handleInput()
    {
        while (Mouse.next())
        {
            handleMouseInput();
        }

        while (Keyboard.next())
        {
            handleKeyboardInput();
        }

    }

    public void handleMouseInput()
    {
        int var1;
        int var2;
        if (Mouse.getEventButtonState())
        {
            var1 = Mouse.getEventX() * width / mc.displayWidth;
            var2 = height - Mouse.getEventY() * height / mc.displayHeight - 1;
            mouseClicked(var1, var2, Mouse.getEventButton());
        }
        else
        {
            var1 = Mouse.getEventX() * width / mc.displayWidth;
            var2 = height - Mouse.getEventY() * height / mc.displayHeight - 1;
            mouseMovedOrUp(var1, var2, Mouse.getEventButton());
        }

    }

    public void handleKeyboardInput()
    {
        if (Keyboard.getEventKeyState())
        {
            if (Keyboard.getEventKey() == Keyboard.KEY_F11)
            {
                mc.toggleFullscreen();
                return;
            }

            keyTyped(Keyboard.getEventCharacter(), Keyboard.getEventKey());
        }

    }

    public virtual void updateScreen()
    {
    }

    public virtual void onGuiClosed()
    {
    }

    public void drawDefaultBackground()
    {
        drawWorldBackground(0);
    }

    public void drawWorldBackground(int var1)
    {
        if (mc.world != null)
        {
            drawGradientRect(0, 0, width, height, 0xC0101010, 0xD0101010);
        }
        else
        {
            drawBackground(var1);
        }

    }

    public void drawBackground(int var1)
    {
        GLManager.GL.Disable(EnableCap.Lighting);
        GLManager.GL.Disable(EnableCap.Fog);
        Tessellator var2 = Tessellator.instance;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/background.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        float var3 = 32.0F;
        var2.startDrawingQuads();
        var2.setColorOpaque_I(4210752);
        var2.addVertexWithUV(0.0D, height, 0.0D, 0.0D, (double)(height / var3 + var1));
        var2.addVertexWithUV(width, height, 0.0D, (double)(width / var3), (double)(height / var3 + var1));
        var2.addVertexWithUV(width, 0.0D, 0.0D, (double)(width / var3), 0 + var1);
        var2.addVertexWithUV(0.0D, 0.0D, 0.0D, 0.0D, 0 + var1);
        var2.draw();
    }

    public virtual bool doesGuiPauseGame()
    {
        return true;
    }

    public virtual void deleteWorld(bool var1, int var2)
    {
    }

    public virtual void selectNextField()
    {
    }
}