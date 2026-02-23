namespace BetaSharp.Stats;

public class StatBasic : StatBase
{
    public StatBasic(int id, string statName, Func<int, string> formatter) : base(id, statName, formatter) { }

    public StatBasic(int id, string statName) : base(id, statName) { }

    public override StatBase RegisterStat()
    {
        base.RegisterStat();
        Stats.GENERAL_STATS.Add(this);
        return this;
    }
}