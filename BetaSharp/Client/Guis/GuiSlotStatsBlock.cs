using BetaSharp.Client.Rendering.Core;
using BetaSharp.Stats;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiSlotStatsBlock : GuiSlotStats
{
    public readonly GuiStats parentStatsGui;


    public GuiSlotStatsBlock(GuiStats parent) : base(parent)
    {
        parentStatsGui = parent;
        field_27273_c = new ArrayList();
        Iterator iterator = Stats.Stats.BLOCKS_MINED_STATS.iterator();

        while (iterator.hasNext())
        {
            StatCrafting stat = (StatCrafting)iterator.next();
            bool hasStat = false;
            int id = stat.func_25072_b();
            if (GuiStats.func_27142_c(parent).writeStat(stat) > 0)
            {
                hasStat = true;
            }
            else if (Stats.Stats.USED[id] != null && GuiStats.func_27142_c(parent).writeStat(Stats.Stats.USED[id]) > 0)
            {
                hasStat = true;
            }
            else if (Stats.Stats.CRAFTED[id] != null && GuiStats.func_27142_c(parent).writeStat(Stats.Stats.CRAFTED[id]) > 0)
            {
                hasStat = true;
            }

            if (hasStat)
            {
                field_27273_c.add(stat);
            }
        }

        field_27272_d = new SorterStatsBlock(this, parent);
    }

    protected override void func_27260_a(int x, int y, Tessellator tessellator)
    {
        base.func_27260_a(x, y, tessellator);
        if (field_27268_b == 0)
        {
            GuiStats.func_27128_a(parentStatsGui, x + 115 - 18 + 1, y + 1 + 1, 18, 18);
        }
        else
        {
            GuiStats.func_27128_a(parentStatsGui, x + 115 - 18, y + 1, 18, 18);
        }

        if (field_27268_b == 1)
        {
            GuiStats.func_27128_a(parentStatsGui, x + 165 - 18 + 1, y + 1 + 1, 36, 18);
        }
        else
        {
            GuiStats.func_27128_a(parentStatsGui, x + 165 - 18, y + 1, 36, 18);
        }

        if (field_27268_b == 2)
        {
            GuiStats.func_27128_a(parentStatsGui, x + 215 - 18 + 1, y + 1 + 1, 54, 18);
        }
        else
        {
            GuiStats.func_27128_a(parentStatsGui, x + 215 - 18, y + 1, 54, 18);
        }

    }

    protected override void drawSlot(int index, int x, int y, int rowHeight, Tessellator tessellator)
    {
        StatCrafting stat = func_27264_b(index);
        int id = stat.func_25072_b();
        GuiStats.func_27148_a(parentStatsGui, x + 40, y, id);
        func_27265_a((StatCrafting)Stats.Stats.CRAFTED[id], x + 115, y, index % 2 == 0);
        func_27265_a((StatCrafting)Stats.Stats.USED[id], x + 165, y, index % 2 == 0);
        func_27265_a(stat, x + 215, y, index % 2 == 0);
    }

    protected override string func_27263_a(int column)
    {
        return column == 0 ? "stat.crafted" : column == 1 ? "stat.used" : "stat.mined";
    }
}