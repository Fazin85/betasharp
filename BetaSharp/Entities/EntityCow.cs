//using BetaSharp.Client.Rendering.Entities;
//using BetaSharp.Client.Rendering.Entities.Models;
using BetaSharp.Items;
using BetaSharp.Worlds;

namespace BetaSharp.Entities;

public class EntityCow : EntityAnimal
{
    public EntityCow(World world) : base(world)
    {
        this.texture = "/mob/cow.png";
        this.setBoundingBoxSpacing(0.9F, 1.3F);
        //Model = new ModelCow();
        //customRenderer = new CowEntityRenderer(Model, 1.3f);
    }

    protected override string getLivingSound()
    {
        return "mob.cow";
    }

    protected override string getHurtSound()
    {
        return "mob.cowhurt";
    }

    protected override string getDeathSound()
    {
        return "mob.cowhurt";
    }

    protected override float getSoundVolume()
    {
        return 0.4F;
    }

    protected override int getDropItemId()
    {
        return Item.Leather.id;
    }

    public override bool interact(EntityPlayer player)
    {
        ItemStack heldBucket = player.inventory.getSelectedItem();
        if (heldBucket != null && heldBucket.itemId == Item.Bucket.id)
        {
            player.inventory.setStack(player.inventory.selectedSlot, new ItemStack(Item.MilkBucket));
            return true;
        }
        else
        {
            return false;
        }
    }
}
