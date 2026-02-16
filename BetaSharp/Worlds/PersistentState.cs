using BetaSharp.NBT;

namespace BetaSharp.Worlds;

public abstract class PersistentState : java.lang.Object
{
    public readonly string Id;
    private bool IsDirty;

    public PersistentState(JString var1)
    {
        Id = var1.value;
    }

    // Lowercase alias for compatibility
    public string id => Id;

    public abstract void readNBT(NBTTagCompound var1);

    public abstract void writeNBT(NBTTagCompound var1);

    public void markDirty()
    {
        setDirty(true);
    }

    public void setDirty(bool IsDirty)
    {
        this.IsDirty = IsDirty;
    }

    public bool isDirty()
    {
        return IsDirty;
    }
}