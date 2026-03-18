using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemSoup : ItemFood
{

    public ItemSoup(int id, int healAmount) : base(id, healAmount, false)
    {
    }

    public override ItemStack AltFire(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
        base.AltFire(itemStack, world, entityPlayer);
        return new ItemStack(Item.Bowl);
    }
}