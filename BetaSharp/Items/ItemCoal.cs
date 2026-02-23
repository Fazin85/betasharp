namespace BetaSharp.Items;

public class ItemCoal : Item
{

    public ItemCoal(int id) : base(id)
    {
        setHasSubtypes(true);
        setMaxDamage(0);
    }

    public override String getItemNameIS(ItemStack itemStack)
    {
        return itemStack.getDamage() == 1 ? "item.charcoal" : "item.coal";
    }
}
