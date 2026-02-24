using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiGameOver : GuiScreen
{
    public override bool PausesGame => false;

    public GuiGameOver()
    {
        int buttonLeft = Width / 2 - 100;
        int buttonTop = Height / 4 + 72;
        Button respawnButton = new(buttonLeft, buttonTop, "Respawn") { Enabled = mc.session != null };
        Button titleButton = new(buttonLeft, buttonTop + 24, "Title menu") { Enabled = mc.session != null };
        respawnButton.Clicked += (_, _) =>
        {
            mc.player.respawn();
            mc.OpenScreen(null);
        };
        titleButton.Clicked += (_, _) =>
        {
            mc.changeWorld(null);
            mc.OpenScreen(new GuiMainMenu());
        };
        Children.AddRange([respawnButton, titleButton]);
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        Gui.DrawGradientRect(0, 0, Width, Height, 0x60500000, 0xA0803030);
        GLManager.GL.PushMatrix();
        GLManager.GL.Scale(2.0F, 2.0F, 2.0F);
        Gui.DrawCenteredString(FontRenderer, "Game over!", Width / 2 / 2, 30, 0xFFFFFF);
        GLManager.GL.PopMatrix();
        Gui.DrawCenteredString(FontRenderer, "Score: &e" + mc.player.getScore(), Width / 2, 100, 0xFFFFFF);
    }
}
