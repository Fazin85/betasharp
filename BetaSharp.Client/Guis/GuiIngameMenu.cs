using BetaSharp.Stats;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Guis;

public class GuiIngameMenu : Screen
{
    private int _saveStepTimer = 0;
    private int _menuTickCounter = 0;

    public GuiIngameMenu()
    {
        _saveStepTimer = 0;
        Children.Clear();

        int verticalOffset = -16;
        int centerX = Width / 2;
        int centerY = Height / 4;
        int buttonLeft = centerX - 100;

        string quitText = (MC.isMultiplayerWorld() && MC.internalServer == null) ? "Disconnect" : "Save and quit to title";

        Button backToGameButton = new(buttonLeft, centerY + 24 + verticalOffset, "Back to game");
        Button achievementsButton = new(buttonLeft, centerY + 48 + verticalOffset, StatCollector.TranslateToLocal("gui.achievements"))
        {
            Size = new(98, 20),
        };
        Button statsButton = new(centerX + 2, centerY + 48 + verticalOffset, StatCollector.TranslateToLocal("gui.stats"))
        {
            Size = new(98, 20),
        };
        Button optionsButton = new(buttonLeft, centerY + 96 + verticalOffset, "Options...");
        Button quitButton = new(buttonLeft, centerY + 100 + verticalOffset, quitText);

        backToGameButton    .Clicked += (_, _) => MC.OpenScreen(null);
        achievementsButton  .Clicked += (_, _) => MC.OpenScreen(new GuiAchievements(MC.statFileWriter));
        statsButton         .Clicked += (_, _) => MC.OpenScreen(new GuiStats(this, MC.statFileWriter));
        optionsButton       .Clicked += (_, _) => MC.OpenScreen(new GuiOptions(this, MC.options));
        quitButton          .Clicked += QuitClicked;

        AddChildren(quitButton, backToGameButton, optionsButton, achievementsButton, statsButton);
    }

    private void QuitClicked(object? o, MouseEventArgs e)
    {
        MC.statFileWriter.WriteStat(Stats.Stats.LeaveGameStat, 1);
        if (MC.isMultiplayerWorld())
        {
            MC.world.Disconnect();
        }

        MC.stopInternalServer();
        MC.changeWorld(null);
        MC.OpenScreen(new GuiMainMenu());
    }

    public override void UpdateScreen()
    {
        ++_menuTickCounter;
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();

        bool isSavingActive = !MC.world.attemptSaving(_saveStepTimer++);

        if (isSavingActive || _menuTickCounter < 20)
        {
            float pulse = (_menuTickCounter % 10 + e.TickDelta) / 10.0F;
            pulse = MathHelper.Sin(pulse * (float)Math.PI * 2.0F) * 0.2F + 0.8F;
            int color = (int)(255.0F * pulse);
            Gui.DrawString(FontRenderer, "Saving level..", 8, Height - 16, (uint)(color << 16 | color << 8 | color));
        }

        Gui.DrawCenteredString(FontRenderer, "Game menu", Width / 2, 40, 0xFFFFFF);
    }
}
