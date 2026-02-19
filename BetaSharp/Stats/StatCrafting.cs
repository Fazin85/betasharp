namespace BetaSharp.Stats;

public class StatCrafting : StatBase
{

    private readonly int itemId;

    public StatCrafting(int var1, String var2, int itemId) : base(var1, var2)
    {
        this.itemId = itemId;
    }

    public int getItemId()
    {
        return itemId;
    }
}