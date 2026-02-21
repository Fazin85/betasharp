using System.Globalization;
using BetaSharp.Stats.Achievements; 

namespace BetaSharp.Stats;

public class StatBase
{
    public readonly int Id;
    public readonly string Name;
    public bool LocalOnly { get; private set; }
    public string? StatGuid { get; private set; }
    private readonly Func<int, string> _formatter;

    public static readonly Func<int, string> IntegerFormat = (val) => val.ToString("N0", CultureInfo.InvariantCulture);
    public static readonly Func<int, string> TimeFormat = (val) => {
        double seconds = val / 20.0;
        double minutes = seconds / 60.0;
        double hours = minutes / 60.0;
        double days = hours / 24.0;

        if (days > 0.5) return days.ToString("F2") + " d";
        if (hours > 0.5) return hours.ToString("F2") + " h";
        if (minutes > 0.5) return minutes.ToString("F2") + " m";
        return seconds.ToString("F2") + " s";
    };
    public static readonly Func<int, string> DistanceFormat = (val) => {
        double meters = val / 100.0;
        double kilometers = meters / 1000.0;
        if (kilometers > 0.5) return kilometers.ToString("F2") + " km";
        return meters.ToString("F2") + " m";
    };

    public StatBase(int id, string name, Func<int, string> formatter)
    {
        Id = id;
        Name = name;
        _formatter = formatter;
        LocalOnly = false;
    }

    public StatBase(int id, string name) : this(id, name, IntegerFormat) { }

    public virtual StatBase SetLocalOnly()
    {
        LocalOnly = true;
        return this;
    }

    public virtual StatBase RegisterStat()
    {
        if (Stats.IdToStat.ContainsKey(Id))
        {
            throw new InvalidOperationException($"Duplicate stat id: \"{Stats.IdToStat[Id].Name}\" and \"{Name}\" at id {Id}");
        }
        
        Stats.AllStats.Add(this);
        Stats.IdToStat.Add(Id, this);
        StatGuid = AchievementMap.getGuid(Id);
        
        return this;
    }

    public virtual bool IsAchievement() => false;

    public string Format(int value) => _formatter(value);

    public override string ToString() => Name;
}