using BetaSharp.Client.Guis;
using BetaSharp.Stats;

namespace BetaSharp.Client.Util;

/// <summary>
/// Helper class that encapsulates the repeated logic for comparing stats in comparators.
/// Eliminates code duplication between SorterStatsItem and SorterStatsBlock.
/// </summary>
public static class StatsComparerHelper
{
    /// <summary>
    /// Compares two statistic values with null handling.
    /// </summary>
    /// <param name="stat1">The first statistic value</param>
    /// <param name="stat2">The second statistic value</param>
    /// <param name="guiStats">The GUI context for reading values</param>
    /// <param name="multiplier">Multiplier for the result (typically sort direction)</param>
    /// <returns>The comparison result or null if both are null</returns>
    public static int? CompareStats(StatBase stat1, StatBase stat2, GuiStats guiStats, int multiplier = 1)
    {
        if (stat1 != null || stat2 != null)
        {
            // If one of the stats is null, return appropriate order
            if (stat1 == null)
                return 1;

            if (stat2 == null)
                return -1;

            // Compare the values of both stats
            int value1 = GuiStats.func_27142_c(guiStats).writeStat(stat1);
            int value2 = GuiStats.func_27142_c(guiStats).writeStat(stat2);
            
            if (value1 != value2)
                return (value1 - value2) * multiplier;
        }

        return null; // Both values are null, continue with fallback logic
    }
}
