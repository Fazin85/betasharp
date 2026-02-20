using BetaSharp.Client.Rendering.Core;
using BetaSharp.Stats;

namespace BetaSharp.Client.Guis;

public class GuiSlotStatsGeneral : GuiSlot
{
    readonly GuiStats parentStatsGui;


    public GuiSlotStatsGeneral(GuiStats parent) : base(parent.mc, parent.Width, parent.Height, 32, parent.Height - 64, 10)
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

    protected override bool isSelected(int slotIndex)
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
        parentStatsGui.FontRenderer.DrawStringWithShadow(stat.statName, x + 2, y + 1, index % 2 == 0 ? 0xFFFFFFu : 0x909090u);
        string formatted = stat.format(parentStatsGui.statFileWriter.writeStat(stat));
        parentStatsGui.FontRenderer.DrawStringWithShadow(formatted, x + 2 + 213 - parentStatsGui.FontRenderer.GetStringWidth(formatted), y + 1, index % 2 == 0 ? 0xFFFFFF : 0x909090u);
    }
}
