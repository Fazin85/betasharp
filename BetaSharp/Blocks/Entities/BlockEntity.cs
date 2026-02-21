using BetaSharp.NBT;
using BetaSharp.Network.Packets;
using BetaSharp.Worlds;
using java.lang;
using java.util;
using Exception = java.lang.Exception;

namespace BetaSharp.Blocks.Entities;

public class BlockEntity
{
    private static readonly Dictionary<string, Type> idToClass = new ();
    private static readonly Dictionary<Type, string> classToId = new ();
    public World world;
    public int x;
    public int y;
    public int z;
    protected bool removed;

    private static void Create(Type blockEntityClass, string id)
    {
        if (idToClass.ContainsKey(id))
        {
            throw new IllegalArgumentException("Duplicate id: " + id);
        }
        else
        {
            idToClass.Add(id, blockEntityClass);
            classToId.Add(blockEntityClass, id);
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
        string entityId = (string)classToId[(GetType())];
        if (entityId == null)
        {
            throw new RuntimeException(GetType() + " is missing a mapping! This is a bug!");
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
            Type blockEntityClass = (Type)idToClass[(nbt.GetString("id"))];
            if (blockEntityClass != null)
            {
                blockEntity = ((BlockEntity)Activator.CreateInstance(blockEntityClass));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception);
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
        Create(typeof(BlockEntityFurnace), "Furnace");
        Create(typeof(BlockEntityChest), "Chest");
        Create(typeof(BlockEntityRecordPlayer), "RecordPlayer");
        Create(typeof(BlockEntityDispenser), "Trap");
        Create(typeof(BlockEntitySign), "Sign");
        Create(typeof(BlockEntityMobSpawner), "MobSpawner");
        Create(typeof(BlockEntityNote), "Music");
        Create(typeof(BlockEntityPiston), "Piston");
    }
}
