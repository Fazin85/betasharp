using BetaSharp.Client.Rendering.Core;
using BetaSharp.Stats;

namespace BetaSharp.Client.Guis;

public class GuiSlotStatsGeneral : GuiSlot
{
    readonly GuiStats parentStatsGui;


    public GuiSlotStatsGeneral(GuiStats parent) : base(GuiStats.func_27141_a(parent), parent.width, parent.height, 32, parent.height - 64, 10)
    {
        parentStatsGui = parent;
        func_27258_a(false);
    }

    public override int getSize()
    {
        return Stats.Stats.GENERAL_STATS.size();
    }

    protected override void elementClicked(int var1, bool var2)
    {
    }

    protected override bool isSelected(int var1)
    {
        return false;
    }

    protected override int getContentHeight()
    {
        return getSize() * 10;
    }

    protected override void drawBackground()
    {
        parentStatsGui.drawDefaultBackground();
    }

    protected override void drawSlot(int index, int x, int y, int rowHeight, Tessellator tessellator)
    {
        StatBase stat = (StatBase)Stats.Stats.GENERAL_STATS.get(index);
        parentStatsGui.drawString(GuiStats.func_27145_b(parentStatsGui), stat.statName, x + 2, y + 1, index % 2 == 0 ? 16777215 : 9474192);
        string formatted = stat.format(GuiStats.func_27142_c(parentStatsGui).writeStat(stat));
        parentStatsGui.drawString(GuiStats.func_27140_d(parentStatsGui), formatted, x + 2 + 213 - GuiStats.func_27146_e(parentStatsGui).getStringWidth(formatted), y + 1, index % 2 == 0 ? 16777215 : 9474192);
    }
}