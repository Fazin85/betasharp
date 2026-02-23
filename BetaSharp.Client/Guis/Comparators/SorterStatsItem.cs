using BetaSharp.Stats;

namespace BetaSharp.Client.Guis.Comparators;

public class SorterStatsItem(GuiSlotStatsItem slotStats, GuiStats stats) : IComparer<StatCrafting>
{
    public int Compare(StatCrafting? x, StatCrafting? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        int idX = x.ItemId;
        int idY = y.ItemId;

        StatBase? statX = slotStats.ActiveStatType switch
        {
            0 => Stats.Stats.Broken[idX],
            1 => Stats.Stats.Crafted[idX],
            2 => Stats.Stats.Used[idX],
            _ => null
        };

        StatBase? statY = slotStats.ActiveStatType switch
        {
            0 => Stats.Stats.Broken[idY],
            1 => Stats.Stats.Crafted[idY],
            2 => Stats.Stats.Used[idY],
            _ => null
        };

        if (statX is not null || statY is not null)
        {
            if (statX is null) return 1;
            if (statY is null) return -1;

            int valueX = stats.statFileWriter.GetStatValue(statX);
            int valueY = stats.statFileWriter.GetStatValue(statY);

            if (valueX != valueY)
            {
                return (valueX - valueY) * slotStats.SortOrder;
            }
        }

        return idX - idY;
    }
}
