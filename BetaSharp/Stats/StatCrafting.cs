namespace BetaSharp.Stats;

public class StatCrafting : StatBase
{

    private readonly int field_25073_a;

    public StatCrafting(int var1, String var2, int var3) : base(var1, var2)
    {
        field_25073_a = var3;
    }

    public int func_25072_b()
    {
        return field_25073_a;
    }
}