using BetaSharp.NBT;
using java.lang;
using java.util;

namespace BetaSharp.Worlds.Storage;

public class PersistentStateManager : java.lang.Object
{
    private readonly WorldStorage saveHandler;
    private readonly Map loadedDataMap = new HashMap();
    private readonly List loadedDataList = new ArrayList();
    private readonly Map idCounts = new HashMap();
    private static readonly Class c = ikvm.runtime.Util.getClassFromTypeHandle(typeof(java.lang.String).TypeHandle);

    public PersistentStateManager(WorldStorage var1)
    {
        saveHandler = var1;
        loadIdCounts();
    }

    public PersistentState loadData(Class var1, string var2)
    {
        PersistentState var3 = (PersistentState)loadedDataMap.get(var2);
        if (var3 != null)
        {
            return var3;
        }

        if (saveHandler != null)
        {
            try
            {
                java.io.File file = saveHandler.getWorldPropertiesFile(var2);
                if (file != null && file.exists())
                {
                    try
                    {
                        var3 = (PersistentState)var1.getConstructor(c).newInstance(var2);
                    }
                    catch (java.lang.Exception e)
                    {
                        throw new RuntimeException("Failed to instantiate " + var1.toString(), e);
                    }

                    using var stream = File.OpenRead(file.getAbsolutePath());
                    NBTTagCompound var6 = NbtIo.ReadCompressed(stream);
                    var3.readNBT(var6.GetCompoundTag("data"));
                }
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }
        }

        if (var3 != null)
        {
            loadedDataMap.put(var2, var3);
            loadedDataList.add(var3);
        }

        return var3;
    }

    public void setData(string var1, PersistentState var2)
    {
        if (var2 == null)
        {
            throw new RuntimeException("Can\'t set null data");
        }
        else
        {
            if (loadedDataMap.containsKey(var1))
            {
                loadedDataList.remove(loadedDataMap.remove(var1));
            }

            loadedDataMap.put(var1, var2);
            loadedDataList.add(var2);
        }
    }

    public void saveAllData()
    {
        for (int var1 = 0; var1 < loadedDataList.size(); ++var1)
        {
            PersistentState var2 = (PersistentState)loadedDataList.get(var1);
            if (var2.isDirty())
            {
                saveData(var2);
                var2.setDirty(false);
            }
        }

    }

    private void saveData(PersistentState var1)
    {
        if (saveHandler != null)
        {
            try
            {
                java.io.File file = saveHandler.getWorldPropertiesFile(var1.id);
                if (file != null)
                {
                    NBTTagCompound var3 = new();
                    var1.writeNBT(var3);
                    NBTTagCompound tag = new();
                    tag.SetCompoundTag("data", var3);


                    using var stream = File.OpenWrite(file.getAbsolutePath());
                    NbtIo.WriteCompressed(tag, stream);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
            }

        }
    }

    private void loadIdCounts()
    {
        try
        {
            idCounts.clear();
            if (saveHandler == null)
            {
                return;
            }

            java.io.File file = saveHandler.getWorldPropertiesFile("idcounts");
            if (file != null && file.exists())
            {
                using var stream = File.OpenRead(file.getAbsolutePath());
                NBTTagCompound var3 = NbtIo.Read(stream);

                foreach (var var5 in var3.Values)
                {
                    if (var5 is NBTTagShort)
                    {
                        NBTTagShort var6 = (NBTTagShort)var5;
                        string var7 = var6.Key;
                        short var8 = var6.Value;
                        idCounts.put(var7, Short.valueOf(var8));
                    }
                }
            }
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }

    }

    public int getUniqueDataId(string var1)
    {
        Short var2 = (Short)idCounts.get(var1);
        if (var2 == null)
        {
            var2 = Short.valueOf(0);
        }
        else
        {
            var2 = Short.valueOf((short)(var2.shortValue() + 1));
        }

        idCounts.put(var1, var2);
        if (saveHandler == null)
        {
            return var2.shortValue();
        }
        else
        {
            try
            {
                java.io.File file = saveHandler.getWorldPropertiesFile("idcounts");
                if (file != null)
                {
                    NBTTagCompound tag = new();
                    Iterator var5 = idCounts.keySet().iterator();

                    while (var5.hasNext())
                    {
                        string var6 = (string)var5.next();
                        short var7 = ((Short)idCounts.get(var6)).shortValue();
                        tag.SetShort(var6, var7);
                    }

                    using var stream = File.OpenWrite(file.getAbsolutePath());
                    NbtIo.Write(tag, stream);
                }
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }

            return var2.shortValue();
        }
    }
}
