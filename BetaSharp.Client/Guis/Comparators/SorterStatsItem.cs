using BetaSharp.Stats;

namespace BetaSharp.Client.Guis.Comparators;

public class SorterStatsItem(GuiSlotStatsItem slotStats, GuiStats stats) : IComparer<StatCrafting>
{
    public int Compare(StatCrafting? x, StatCrafting? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        int idX = x.getItemId();
        int idY = y.getItemId();

        StatBase? statX = slotStats.field_27271_e switch
        {
            0 => Stats.Stats.BROKEN[idX],
            1 => Stats.Stats.CRAFTED[idX],
            2 => Stats.Stats.USED[idX],
            _ => null
        };

        StatBase? statY = slotStats.field_27271_e switch
        {
            0 => Stats.Stats.BROKEN[idY],
            1 => Stats.Stats.CRAFTED[idY],
            2 => Stats.Stats.USED[idY],
            _ => null
        };

        if (statX is not null || statY is not null)
        {
            if (statX is null) return 1;
            if (statY is null) return -1;

            int valueX = stats.statFileWriter.writeStat(statX);
            int valueY = stats.statFileWriter.writeStat(statY);

            if (valueX != valueY)
            {
                return (valueX - valueY) * slotStats.field_27270_f;
            }
        }

        return idX - idY;
    }
}
