using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.lang;

namespace BetaSharp.Worlds.Chunks;

public class EmptyChunk : Chunk
{
    public EmptyChunk(World world, int x, int z) : base(world, x, z)
    {
        empty = true;
    }

    public EmptyChunk(World world, byte[] blocks, int x, int z) : base(world, blocks, x, z)
    {
        empty = true;
    }

    public override bool chunkPosEquals(int x, int z)
    {
        return x == this.x && z == this.z;
    }

    public override int getHeight(int var1, int var2)
    {
        return 0;
    }

    public override void populateLight()
    {
    }

    public override void populateHeightMapOnly()
    {
    }

    public override void populateHeightMap()
    {
    }

    public override void populateBlockLight()
    {
    }

    public override int getBlockId(int var1, int var2, int var3)
    {
        return 0;
    }

    public override bool setBlock(int var1, int var2, int var3, int var4, int var5)
    {
        return true;
    }

    public override bool setBlock(int var1, int var2, int var3, int var4)
    {
        return true;
    }

    public override int getBlockMeta(int var1, int var2, int var3)
    {
        return 0;
    }

    public override void setBlockMeta(int var1, int var2, int var3, int var4)
    {
    }

    public override int getLight(LightType var1, int var2, int var3, int var4)
    {
        return 0;
    }

    public override void setLight(LightType var1, int var2, int var3, int var4, int var5)
    {
    }

    public override int getLight(int var1, int var2, int var3, int var4)
    {
        return 0;
    }

    public override void addEntity(Entity var1)
    {
    }

    public override void removeEntity(Entity var1)
    {
    }

    public override void removeEntity(Entity var1, int var2)
    {
    }

    public override bool isAboveMaxHeight(int var1, int var2, int var3)
    {
        return false;
    }

    public override BlockEntity getBlockEntity(int var1, int var2, int var3)
    {
        return null;
    }

    public override void addBlockEntity(BlockEntity var1)
    {
    }

    public override void setBlockEntity(int var1, int var2, int var3, BlockEntity var4)
    {
    }

    public override void removeBlockEntityAt(int var1, int var2, int var3)
    {
    }

    public override void load()
    {
    }

    public override void unload()
    {
    }

    public override void markDirty()
    {
    }

    public override void collectOtherEntities(Entity var1, Box var2, List<Entity> var3)
    {
    }

    public override void collectEntitiesByClass(Class var1, Box var2, List<Entity> var3)
    {
    }

    public override bool shouldSave(bool var1)
    {
        return false;
    }

    public override int loadFromPacket(byte[] var1, int var2, int var3, int var4, int var5, int var6, int var7, int var8)
    {
        int var9 = var5 - var2;
        int var10 = var6 - var3;
        int var11 = var7 - var4;
        int var12 = var9 * var10 * var11;
        return var12 + var12 / 2 * 3;
    }

    public override JavaRandom getSlimeRandom(long var1)
    {
        return new(world.getSeed() + x * x * 4987142 + x * 5947611 + z * z * 4392871L + z * 389711 ^ var1);
    }

    public override bool isEmpty()
    {
        return true;
    }
}