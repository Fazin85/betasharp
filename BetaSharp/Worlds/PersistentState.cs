using BetaSharp.NBT;

namespace BetaSharp.Worlds;

public abstract class PersistentState : java.lang.Object
{
    public readonly string id;
    private bool dirty;

    public PersistentState(string var1)
    {
        id = var1;
    }

    public abstract void readNBT(NBTTagCompound var1);

    public abstract void writeNBT(NBTTagCompound var1);

    public void markDirty()
    {
        setDirty(true);
    }

    public void setDirty(bool var1)
    {
        dirty = var1;
    }

    public bool isDirty()
    {
        return dirty;
    }
}