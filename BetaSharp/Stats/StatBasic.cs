namespace BetaSharp.Stats;

public class StatBasic : StatBase
{
    public StatBasic(int id, string name, Func<int, string> formatter) : base(id, name, formatter) { }

    public StatBasic(int id, string name) : base(id, name) { }

    public override StatBase RegisterStat()
    {
        base.RegisterStat();
        if (!Stats.GeneralStats.Contains(this))
        {
            Stats.GeneralStats.Add(this);
        }
        return this;
    }
}