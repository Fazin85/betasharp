using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiGameOver : GuiScreen
{
    private const int ButtonRespawn = 1;
    private const int ButtonTitle = 2;

    public override bool PausesGame => false;

    public override void InitGui()
    {
        Children.Clear();
        Children.Add(new GuiButton(ButtonRespawn, Width / 2 - 100, Height / 4 + 72, "Respawn"));
        Children.Add(new GuiButton(ButtonTitle, Width / 2 - 100, Height / 4 + 96, "Title menu"));
        if (mc.session == null)
        {
            for (int i = 0; i < Children.Count; ++i)
            {
                GuiButton btn = Children[i];
                if (btn.Id == ButtonRespawn)
                {
                    btn.Enabled = false;
                    break;
                }
            }
        }

    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case ButtonRespawn:
                mc.player.respawn();
                mc.OpenScreen(null);
                break;
            case ButtonTitle:
                mc.changeWorld(null);
                mc.OpenScreen(new GuiMainMenu());
                break;
        }

    }

    protected override void OnRendered(RenderEventArgs e)
    {
        Gui.DrawGradientRect(0, 0, Width, Height, 0x60500000, 0xA0803030);
        GLManager.GL.PushMatrix();
        GLManager.GL.Scale(2.0F, 2.0F, 2.0F);
        Gui.DrawCenteredString(FontRenderer, "Game over!", Width / 2 / 2, 30, 0xFFFFFF);
        GLManager.GL.PopMatrix();
        Gui.DrawCenteredString(FontRenderer, "Score: &e" + mc.player.getScore(), Width / 2, 100, 0xFFFFFF);
        base.OnRendered(mouseX, mouseY, tickDelta);
    }
}
