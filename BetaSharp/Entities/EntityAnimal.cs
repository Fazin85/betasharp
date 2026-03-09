using BetaSharp.Blocks;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Entities;

public abstract class EntityAnimal : EntityCreature, SpawnableEntity
{
    public EntityAnimal(IBlockWorldContext level) : base(level)
    {
    }

    protected override float getBlockPathWeight(int x, int y, int z) => _level.BlocksReader.GetBlockId(x, y - 1, z) == Block.GrassBlock.id ? 10.0F : _level.BlocksReader.GetBrightness(x, y, z) - 0.5F;

    public override void writeNbt(NBTTagCompound nbt) => base.writeNbt(nbt);

    public override void readNbt(NBTTagCompound nbt) => base.readNbt(nbt);

    public override bool canSpawn()
    {
        int x = MathHelper.Floor(this.x);
        int y = MathHelper.Floor(boundingBox.MinY);
        int z = MathHelper.Floor(this.z);
        return _level.BlocksReader.GetBlockId(x, y - 1, z) == Block.GrassBlock.id && _level.BlocksReader.GetBrightness(x, y, z) > 8 && base.canSpawn();
    }

    public override int getTalkInterval() => 120;
}
