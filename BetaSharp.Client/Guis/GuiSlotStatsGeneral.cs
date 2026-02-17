using BetaSharp.Client.Rendering.Core;
using BetaSharp.Stats;

namespace BetaSharp.Client.Guis;

public class GuiSlotStatsGeneral : GuiSlot
{
    readonly GuiStats parentStatsGui;


    public GuiSlotStatsGeneral(GuiStats parent) : base(GuiStats.func_27141_a(parent), parent.Width, parent.Height, 32, parent.Height - 64, 10)
    {
        parentStatsGui = parent;
        func_27258_a(false);
    }

    public override int GetSize()
    {
        return Stats.Stats.GENERAL_STATS.size();
    }

    protected override void ElementClicked(int var1, bool var2)
    {
    }

    protected override bool isSelected(int var1)
    {
        return false;
    }

    protected override int getContentHeight()
    {
        return GetSize() * 10;
    }

    protected override void drawBackground()
    {
        parentStatsGui.DrawDefaultBackground();
    }

    protected override void drawSlot(int index, int x, int y, int rowHeight, Tessellator tessellator)
    {
        StatBase stat = (StatBase)Stats.Stats.GENERAL_STATS.get(index);
        parentStatsGui.DrawString(GuiStats.func_27145_b(parentStatsGui), stat.statName, x + 2, y + 1, index % 2 == 0 ? 0x00FFFFFFu : 0x00909090u);
        string formatted = stat.format(GuiStats.func_27142_c(parentStatsGui).writeStat(stat));
        parentStatsGui.DrawString(GuiStats.func_27140_d(parentStatsGui), formatted, x + 2 + 213 - GuiStats.func_27146_e(parentStatsGui).getStringWidth(formatted), y + 1, index % 2 == 0 ? 0x00FFFFFFu : 0x00909090u);
    }
}