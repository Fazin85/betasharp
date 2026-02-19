using BetaSharp.NBT;
using BetaSharp.Network.Packets;
using BetaSharp.Worlds;
using java.lang;
using java.util;

namespace BetaSharp.Blocks.Entities;

public class BlockEntity : java.lang.Object
{
    public static readonly Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(BlockEntity).TypeHandle);
    private static readonly Map idToClass = new HashMap();
    private static readonly Map classToId = new HashMap();
    public World world;
    public int x;
    public int y;
    public int z;
    protected bool removed;

    private static void create(Class blockEntityClass, string id)
    {
        if (classToId.containsKey(id))
        {
            throw new IllegalArgumentException("Duplicate id: " + id);
        }
        else
        {
            idToClass.put(id, blockEntityClass);
            classToId.put(blockEntityClass, id);
        }
    }

    public virtual void readNbt(NBTTagCompound nbt)
    {
        x = nbt.GetInteger("x");
        y = nbt.GetInteger("y");
        z = nbt.GetInteger("z");
    }

    public virtual void writeNbt(NBTTagCompound nbt)
    {
        string entityId = (string)classToId.get(getClass());
        if (entityId == null)
        {
            throw new RuntimeException(getClass() + " is missing a mapping! This is a bug!");
        }
        else
        {
            nbt.SetString("id", entityId);
            nbt.SetInteger("x", x);
            nbt.SetInteger("y", y);
            nbt.SetInteger("z", z);
        }
    }

    public virtual void tick()
    {
    }

    public static BlockEntity createFromNbt(NBTTagCompound nbt)
    {
        BlockEntity blockEntity = null;

        try
        {
            Class blockEntityClass = (Class)idToClass.get(nbt.GetString("id"));
            if (blockEntityClass != null)
            {
                blockEntity = (BlockEntity)blockEntityClass.newInstance();
            }
        }
        catch (java.lang.Exception exception)
        {
            exception.printStackTrace();
        }

        if (blockEntity != null)
        {
            blockEntity.readNbt(nbt);
        }
        else
        {
            Log.Info("Skipping TileEntity with id " + nbt.GetString("id"));
        }

        return blockEntity;
    }

    public virtual int getPushedBlockData()
    {
        return world.getBlockMeta(x, y, z);
    }

    public void markDirty()
    {
        if (world != null)
        {
            world.updateBlockEntity(x, y, z, this);
        }

    }

    public double distanceFrom(double x, double y, double z)
    {
        double dx = this.x + 0.5D - x;
        double dy = this.y + 0.5D - y;
        double dz = this.z + 0.5D - z;
        return dx * dx + dy * dy + dz * dz;
    }

    public Block getBlock()
    {
        return Block.Blocks[world.getBlockId(x, y, z)];
    }

    public virtual Packet createUpdatePacket()
    {
        return null;
    }

    public bool isRemoved()
    {
        return removed;
    }

    public void markRemoved()
    {
        removed = true;
    }

    public void cancelRemoval()
    {
        removed = false;
    }

    static BlockEntity()
    {
        create(new BlockEntityFurnace().getClass(), "Furnace");
        create(new BlockEntityChest().getClass(), "Chest");
        create(new BlockEntityRecordPlayer().getClass(), "RecordPlayer");
        create(new BlockEntityDispenser().getClass(), "Trap");
        create(new BlockEntitySign().getClass(), "Sign");
        create(new BlockEntityMobSpawner().getClass(), "MobSpawner");
        create(new BlockEntityNote().getClass(), "Music");
        create(new BlockEntityPiston().getClass(), "Piston");
    }
}
