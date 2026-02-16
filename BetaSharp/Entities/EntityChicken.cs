using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Worlds;
using java.lang;

namespace BetaSharp.Entities;

public class EntityChicken : EntityAnimal
{
    public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityChicken).TypeHandle);
    public bool IsWingFlapping = false;
    public float WingRotation = 0.0F;
    public float DestPos = 0.0F;
    public float PreviousDestPos;
    public float PreviousWingRotation;
    public float WingRotationSpeed = 1.0F;
    public int TimeUntilNextEgg;

    public EntityChicken(World World) : base(World)
    {
        texture = "/mob/chicken.png";
        setBoundingBoxSpacing(0.3F, 0.4F);
        health = 4;
        TimeUntilNextEgg = random.nextInt(6000) + 6000;
    }

    public override void tickMovement()
    {
        base.tickMovement();
        if (world.isRemote)
        {
            onGround = System.Math.Abs(y - prevY) < 0.02D;
        }
        PreviousWingRotation = WingRotation;
        PreviousDestPos = DestPos;
        DestPos = (float)((double)DestPos + (double)(onGround ? -1 : 4) * 0.3D);
        if (DestPos < 0.0F)
        {
            DestPos = 0.0F;
        }

        if (DestPos > 1.0F)
        {
            DestPos = 1.0F;
        }

        if (!onGround && WingRotationSpeed < 1.0F)
        {
            WingRotationSpeed = 1.0F;
        }

        WingRotationSpeed = (float)((double)WingRotationSpeed * 0.9D);
        if (!onGround && velocityY < 0.0D)
        {
            velocityY *= 0.6D;
        }

        WingRotation += WingRotationSpeed * 2.0F;
        if (!world.isRemote && --TimeUntilNextEgg <= 0)
        {
            world.playSound(this, "mob.chickenplop", 1.0F, (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
            dropItem(Item.EGG.id, 1);
            TimeUntilNextEgg = random.nextInt(6000) + 6000;
        }

    }

    protected override void onLanding(float FallDistance)
    {
    }

    public override void writeNbt(NBTTagCompound NBT)
    {
        base.writeNbt(NBT);
    }

    public override void readNbt(NBTTagCompound NBT)
    {
        base.readNbt(NBT);
    }

    protected override string getLivingSound()
    {
        return "mob.chicken";
    }

    protected override string getHurtSound()
    {
        return "mob.chickenhurt";
    }

    protected override string getDeathSound()
    {
        return "mob.chickenhurt";
    }

    protected override int getDropItemId()
    {
        return Item.FEATHER.id;
    }
}