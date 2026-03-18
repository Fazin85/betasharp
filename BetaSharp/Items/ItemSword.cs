using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemSword : Item
{

    private int weaponDamage;
    private string swingSound;
    private string slashSound;
    private string klangsound;
    public ItemSword(int id, EnumToolMaterial enumToolMaterial) : base(id)
    {
        maxCount = 1;
        setMaxDamage(enumToolMaterial.getMaxUses());
        weaponDamage = 4 + enumToolMaterial.getDamageVsEntity() * 2;
        slashSound = "item.sword.hit";
        klangsound = "item.sword.swipe_and_klang";
    }

    

    public override float getMiningSpeedMultiplier(ItemStack itemStack, Block block)
    {
        return block.id == Block.Cobweb.id ? 15.0F : 1.5F;
    }

    public override bool postHit(ItemStack itemStack, EntityLiving a, EntityLiving b)
    {
        a.world.playSound(a, slashSound, 1f, 1f);
        itemStack.damageItem(1, b);
        return true;
    }

    public override bool postMine(ItemStack itemStack, int blockId, int x, int y, int z, EntityLiving entityLiving)
    {
        entityLiving.world.playSound(entityLiving, klangsound, 1f, 1f);
        itemStack.damageItem(2, entityLiving);
        return true;
    }

    public override int getAttackDamage(Entity entity)
    {
        return weaponDamage;
    }

    public override bool isHandheld()
    {
        return true;
    }

    public override bool isSuitableFor(Block block)
    {
        return block.id == Block.Cobweb.id;
    }
}
