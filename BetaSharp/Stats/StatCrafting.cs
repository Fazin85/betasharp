namespace BetaSharp.Stats;

public class StatCrafting : StatBase
{
    public int ItemId { get; }

    public StatCrafting(int id, string statName, int itemId) : base(id, statName)
    {
        ItemId = itemId;
    }
}