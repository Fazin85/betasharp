using BetaSharp.Stats.Achievements;
using java.lang;
using java.text;
using java.util;

namespace BetaSharp.Stats;

public class StatBase : java.lang.Object
{
    public readonly int id;
    public readonly string statName;
    public bool localOnly;
    public string statGuid;
    private readonly StatFormatter formatter;
    private static NumberFormat DEFAULT_NUMBER_FORMAT = NumberFormat.getIntegerInstance(Locale.US);
    public static StatFormatter INTEGER_FORMAT = new IntegerStatFormatter();
    private static DecimalFormat DEFAULT_DECIMAL_FORMAT = new DecimalFormat("########0.00");
    public static StatFormatter TIME_PROVIDER = new TimeStatFormatter();
    public static StatFormatter DISTANCE_PROVIDER = new DistanceStatFormatter();

    public StatBase(int var1, string var2, StatFormatter var3)
    {
        localOnly = false;
        id = var1;
        statName = var2;
        formatter = var3;
    }

    public StatBase(int var1, string var2) : this(var1, var2, INTEGER_FORMAT)
    {
    }

    public virtual StatBase setLocalOnly()
    {
        localOnly = true;
        return this;
    }

    public virtual StatBase registerStat()
    {
        if (Stats.ID_TO_STAT.containsKey(Integer.valueOf(id)))
        {
            throw new RuntimeException("Duplicate stat id: \"" + ((StatBase)Stats.ID_TO_STAT.get(Integer.valueOf(id))).statName + "\" and \"" + statName + "\" at id " + id);
        }
        else
        {
            Stats.ALL_STATS.add(this);
            Stats.ID_TO_STAT.put(Integer.valueOf(id), this);
            statGuid = AchievementMap.getGuid(id);
            return this;
        }
    }

    public virtual bool isAchievement()
    {
        return false;
    }

    public string format(int value)
    {
        return formatter.Format(value);
    }

    public override string toString()
    {
        return statName;
    }

    public static NumberFormat defaultNumberFormat()
    {
        return DEFAULT_NUMBER_FORMAT;
    }

    public static DecimalFormat defaultDecimalFormat()
    {
        return DEFAULT_DECIMAL_FORMAT;
    }
}