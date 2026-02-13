using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemFood : Item
{

    private int healAmount;
    private bool isWolfsFavoriteMeat;

    public ItemFood(int id, int healAmount, bool isWolfsFavoriteMeat) : base(id)
    {
        this.healAmount = healAmount;
        this.isWolfsFavoriteMeat = isWolfsFavoriteMeat;
        maxCount = 1;
    }

    public override ItemStack use(ItemStack itemStack, World world, EntityPlayer entityPlayer)
    {
        --itemStack.count;
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