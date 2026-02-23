using BetaSharp.Stats;

namespace BetaSharp.Client.Guis.Comparators;

public class SorterStatsBlock(GuiSlotStatsBlock slotStats, GuiStats stats) : IComparer<StatCrafting>
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
            2 => Stats.Stats.mineBlockStatArray[idX],
            0 => Stats.Stats.CRAFTED[idX],
            1 => Stats.Stats.USED[idX],
            _ => null
        };

        StatBase? statY = slotStats.ActiveStatType switch
        {
            2 => Stats.Stats.mineBlockStatArray[idY],
            0 => Stats.Stats.CRAFTED[idY],
            1 => Stats.Stats.USED[idY],
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
                return (valueX - valueY) * slotStats.SortOrder;
            }
        }
        return idX - idY;
    }
}