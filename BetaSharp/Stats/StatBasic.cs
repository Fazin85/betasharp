namespace BetaSharp.Stats;

public class StatBasic : StatBase
{
    public StatBasic(int var1, string var2, StatFormatter var3) : base(var1, var2, var3)
    {
    }

    public StatBasic(int var1, string var2) : base(var1, var2)
    {
    }

    public override StatBase registerStat()
    {
        base.registerStat();
        Stats.GENERAL_STATS.add(this);
        return this;
    }
}