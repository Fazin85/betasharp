namespace BetaSharp.Items;

public class ItemPiston : ItemBlock
{

    public ItemPiston(int id) : base(id)
    {
    }

    public override int getPlacementMetadata(int meta)
    {
        return 7;
    }
}