using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiGameOver : GuiScreen
{
    private const int BUTTON_RESPAWN = 1;
    private const int BUTTON_TITLE = 2;

    public override void InitGui()
    {
        _controlList.Clear();
        _controlList.Add(new GuiButton(BUTTON_RESPAWN, Width / 2 - 100, Height / 4 + 72, "Respawn"));
        _controlList.Add(new GuiButton(BUTTON_TITLE, Width / 2 - 100, Height / 4 + 96, "Title menu"));
        if (mc.session == null)
        {
            for (int i = 0; i < _controlList.Count; ++i)
            {
                GuiButton btn = _controlList[i];
                if (btn.Id == BUTTON_RESPAWN)
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
            case BUTTON_RESPAWN:
                mc.player.respawn();
                mc.displayGuiScreen(null);
                break;
            case BUTTON_TITLE:
                mc.changeWorld1(null);
                mc.displayGuiScreen(new GuiMainMenu());
                break;
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawGradientRect(0, 0, Width, Height, 0x60500000, 0xA0803030);
        GLManager.GL.PushMatrix();
        GLManager.GL.Scale(2.0F, 2.0F, 2.0F);
        DrawCenteredString(fontRenderer, "Game over!", Width / 2 / 2, 30, 0x00FFFFFF);
        GLManager.GL.PopMatrix();
        DrawCenteredString(fontRenderer, "Score: &e" + mc.player.getScore(), Width / 2, 100, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }

    public override bool DoesGuiPauseGame()
    {
        return false;
    }
}