using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemFood : Item
{

    private int healAmount;
    private bool isWolfsFavoriteMeat;
    private string eatSound;

    public ItemFood(int id, int healAmount, bool isWolfsFavoriteMeat, string eatSound = "random.crunch") : base(id)
    {
        this.healAmount = healAmount;
        this.isWolfsFavoriteMeat = isWolfsFavoriteMeat;
        this.eatSound = eatSound;
        //maxCount = 1;
    }

    public override ItemStack AltFire(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
        if (entityPlayer.isHealthy()) return itemStack;
        --itemStack.count;
        world.playSound(entityPlayer, eatSound, 0.4f, 1f); // le son est juste pas reconnu
        entityPlayer.heal(healAmount);
        return itemStack;
    }

    public int getHealAmount()
    {
        return healAmount;
    }

    public bool getIsWolfsFavoriteMeat()
    {
        return isWolfsFavoriteMeat;
    }
}
