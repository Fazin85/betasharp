using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using java.awt;
using java.awt.datatransfer;
using java.util;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL.Legacy;
using System;
using System.Collections.Generic;

namespace BetaSharp.Client.Guis;

public class GuiScreen : Control
{
    private static readonly ILogger<GuiScreen> s_logger = Log.Instance.For<GuiScreen>();
    internal override bool TopLevel => true;
    public Minecraft mc;
    public bool AllowUserInput = false;
    public virtual bool PausesGame => true;
    public TextRenderer FontRenderer;
    public GuiParticle ParticlesGui;
    private GuiButton SelectedButton = null;
    protected bool _isSubscribedToKeyboard = false;

    protected virtual void KeyTyped(char eventChar, int eventKey)
    {
        if (eventKey == Keyboard.KEY_ESCAPE)
        {
            mc.displayGuiScreen(null);
            mc.setIngameFocus();
        }
    }

    protected virtual void CharTyped(char eventChar) { }

    public static string GetClipboardString()
    {
        unsafe
        {
            if (Display.isCreated())
            {
                return Display.getGlfw().GetClipboardString(Display.getWindowHandle());
            }
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
            s_logger.LogError($"Failed to set clipboard string: {text}");
        }
    }

    public void SetWorldAndResolution(Minecraft mc, int width, int height)
    {
        ParticlesGui = new GuiParticle(mc);
        this.mc = mc;
        FontRenderer = mc.fontRenderer;
        Size = new(width, height);
    }

    public virtual void UpdateScreen() { }

    public virtual void OnGuiClosed()
    {
        if (_isSubscribedToKeyboard)
        {
            Keyboard.OnCharacterTyped -= CharTyped;
            _isSubscribedToKeyboard = false;
        }
    }

    public void DrawDefaultBackground()
    {
        DrawWorldBackground(0);
    }

    public void DrawWorldBackground(int var1)
    {
        if (mc.world != null)
        {
            Gui.DrawGradientRect(0, 0, Width, Height, 0xC0101010, 0xD0101010);
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
        mc.textureManager.BindTexture(mc.textureManager.GetTextureId("/gui/background.png"));
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

    public virtual void DeleteWorld(bool confirmed, int index) { }

    public virtual void SelectNextField()
    {

    }
}
