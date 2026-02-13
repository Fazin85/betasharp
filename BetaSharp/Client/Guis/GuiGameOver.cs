using BetaSharp.Client.Rendering.Core;

namespace BetaSharp.Client.Guis;

public class GuiGameOver : GuiScreen
{
    private const int BUTTON_RESPAWN = 1;
    private const int BUTTON_TITLE = 2;

    public override void initGui()
    {
        controlList.clear();
        controlList.add(new GuiButton(BUTTON_RESPAWN, width / 2 - 100, height / 4 + 72, "Respawn"));
        controlList.add(new GuiButton(BUTTON_TITLE, width / 2 - 100, height / 4 + 96, "Title menu"));
        if (mc.session == null)
        {
            for (int i = 0; i < controlList.size(); ++i)
            {
                GuiButton btn = (GuiButton)controlList.get(i);
                if (btn.id == BUTTON_RESPAWN)
                {
                    btn.enabled = false;
                    break;
                }
            }
        }

    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
    }

    protected override void actionPerformed(GuiButton button)
    {
        switch (button.id)
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

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawGradientRect(0, 0, width, height, 1615855616, -1602211792);
        GLManager.GL.PushMatrix();
        GLManager.GL.Scale(2.0F, 2.0F, 2.0F);
        drawCenteredString(fontRenderer, "Game over!", width / 2 / 2, 30, 16777215);
        GLManager.GL.PopMatrix();
        drawCenteredString(fontRenderer, "Score: &e" + mc.player.getScore(), width / 2, 100, 16777215);
        base.render(mouseX, mouseY, partialTicks);
    }

    public override bool doesGuiPauseGame()
    {
        return false;
    }
}