using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemBow : Item
{

    public ItemBow(int id) : base(id)
    {
        maxCount = 1;
    }

    public override ItemStack use(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
        if (entityPlayer.inventory.consumeInventoryItem(Item.ARROW.id))
        {
            world.playSound(entityPlayer, "random.bow", 1.0F, 1.0F / (itemRand.NextFloat() * 0.4F + 0.8F));
            if (!world.isRemote)
            {
                world.SpawnEntity(new EntityArrow(world, entityPlayer));
            }
        }

        return itemStack;
    }
}