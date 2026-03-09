using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Entities;

public class EntityMonster : EntityCreature, Monster
{
    protected int attackStrength = 2;

    public EntityMonster(IBlockWorldContext world) : base(world) => health = 20;

    public override void tickMovement()
    {
        float brightness = getBrightnessAtEyes(1.0F);
        if (brightness > 0.5F)
        {
            entityAge += 2;
        }

        base.tickMovement();
    }

    public override void tick()
    {
        base.tick();
        if (!_level.IsRemote && _level.Difficulty == 0)
        {
            markDead();
        }
    }

    protected override Entity? findPlayerToAttack()
    {
        EntityPlayer? player = _level.Entities.GetClosestPlayer(x, y, z, 16.0D);
        return player != null && canSee(player) ? player : null;
    }

    public override bool damage(Entity? entity, int amount)
    {
        if (base.damage(entity, amount))
        {
            if (passenger != entity && vehicle != entity)
            {
                if (entity != this)
                {
                    playerToAttack = entity;
                }

                return true;
            }

            return true;
        }

        return false;
    }

    protected override void attackEntity(Entity entity, float distance)
    {
        if (attackTime <= 0 && distance < 2.0F && entity.boundingBox.MaxY > boundingBox.MinY && entity.boundingBox.MinY < boundingBox.MaxY)
        {
            attackTime = 20;
            entity.damage(this, attackStrength);
        }
    }

    protected override float getBlockPathWeight(int x, int y, int z) => 0.5F - _level.Lighting.GetLuminance(x, y, z);

    public override void writeNbt(NBTTagCompound nbt) => base.writeNbt(nbt);

    public override void readNbt(NBTTagCompound nbt) => base.readNbt(nbt);

    public override bool canSpawn()
    {
        int x = MathHelper.Floor(this.x);
        int y = MathHelper.Floor(boundingBox.MinY);
        int z = MathHelper.Floor(this.z);
        if (_level.Lighting.GetBrightness(LightType.Sky, x, y, z) > random.NextInt(32))
        {
            return false;
        }

        int lightLevel = _level.Lighting.GetLightLevel(x, y, z);
        if (_level.Environment.IsThundering())
        {
            int ambientDarkness = _level.Environment.AmbientDarkness;
            _level.Environment.AmbientDarkness = 10;
            lightLevel = _level.Lighting.GetLightLevel(x, y, z);
            _level.Environment.AmbientDarkness = ambientDarkness;
        }

        return lightLevel <= random.NextInt(8) && base.canSpawn();
    }
}
