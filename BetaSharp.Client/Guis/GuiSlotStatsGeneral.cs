using BetaSharp.Client.Rendering.Core;
using BetaSharp.Stats;

namespace BetaSharp.Client.Guis;

public class GuiSlotStatsGeneral : GuiSlot
{
    readonly GuiStats parentStatsGui;


    public GuiSlotStatsGeneral(GuiStats parent) : base(parent.mc, parent.width, parent.height, 32, parent.height - 64, 10)
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

    protected override bool isSelected(int slotIndex)
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
        parentStatsGui.drawString(parentStatsGui.fontRenderer, stat.statName, x + 2, y + 1, index % 2 == 0 ? 0xFFFFFFu : 0x909090u);
        string formatted = stat.format(parentStatsGui.statFileWriter.writeStat(stat));
        parentStatsGui.drawString(parentStatsGui.fontRenderer, formatted, x + 2 + 213 - parentStatsGui.fontRenderer.getStringWidth(formatted), y + 1, index % 2 == 0 ? 0x00FFFFFFu : 0x00909090u);
    }
}
