using BetaSharp.Blocks;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Entities;

public class EntityFallingSand : Entity
{
    public int blockId;
    public int fallTime;

    public EntityFallingSand(IBlockWorldContext world) : base(world)
    {
    }

    public EntityFallingSand(IBlockWorldContext world, double x, double y, double z, int blockId) : base(world)
    {
        this.blockId = blockId;
        preventEntitySpawning = true;
        setBoundingBoxSpacing(0.98F, 0.98F);
        standingEyeHeight = height / 2.0F;
        setPosition(x, y, z);
        velocityX = 0.0D;
        velocityY = 0.0D;
        velocityZ = 0.0D;
        prevX = x;
        prevY = y;
        prevZ = z;
    }

    protected override bool bypassesSteppingEffects() => false;


    public override bool isCollidable() => !dead;

    public override void tick()
    {
        if (blockId == 0)
        {
            markDead();
        }
        else
        {
            prevX = x;
            prevY = y;
            prevZ = z;
            ++fallTime;
            velocityY -= 0.04F;
            move(velocityX, velocityY, velocityZ);
            velocityX *= 0.98F;
            velocityY *= 0.98F;
            velocityZ *= 0.98F;
            int floorX = MathHelper.Floor(x);
            int floorY = MathHelper.Floor(y);
            int floorZ = MathHelper.Floor(z);
            if (_level.BlocksReader.GetBlockId(floorX, floorY, floorZ) == blockId)
            {
                _level.BlockWriter.SetBlock(floorX, floorY, floorZ, 0);
            }

            if (onGround)
            {
                velocityX *= 0.7F;
                velocityZ *= 0.7F;
                velocityY *= -0.5D;
                markDead();
                if ((!Block.Blocks[blockId].canPlaceAt(new CanPlaceAtCtx(_level, 0, floorX, floorY, floorZ)) || BlockSand.canFallThrough(new OnTickEvt(_level, floorX, floorY - 1, floorZ, 0, blockId)) || !_level.BlockWriter.SetBlock(floorX, floorY, floorZ, blockId)) && !_level.IsRemote)
                {
                    dropItem(blockId, 1);
                }
            }
            else if (fallTime > 100 && !_level.IsRemote)
            {
                dropItem(blockId, 1);
                markDead();
            }
        }
    }

    public override void writeNbt(NBTTagCompound nbt) => nbt.SetByte("Tile", (sbyte)blockId);

    public override void readNbt(NBTTagCompound nbt) => blockId = nbt.GetByte("Tile") & 255;

    public override float getShadowRadius() => 0.0F;

    public IBlockWorldContext getWorld() => _level;
}
