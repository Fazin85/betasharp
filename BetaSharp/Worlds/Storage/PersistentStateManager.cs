using BetaSharp.NBT;
using java.io;
using java.lang;
using java.util;

namespace BetaSharp.Worlds.Storage;

public class PersistentStateManager : java.lang.Object
{
    private readonly WorldStorage saveHandler;
    private readonly Map loadedDataMap = new HashMap();
    private readonly List loadedDataList = new ArrayList();
    private readonly Map idCounts = new HashMap();
    private static readonly Class c = new JString("").getClass();

    public PersistentStateManager(WorldStorage var1)
    {
        saveHandler = var1;
        loadIdCounts();
    }

    public PersistentState loadData(Class var1, JString var2)
    {
        PersistentState var3 = (PersistentState)loadedDataMap.get(var2);
        if (var3 != null)
        {
            return var3;
        }
        else
        {
            if (saveHandler != null)
            {
                try
                {
                    java.io.File var4 = saveHandler.getWorldPropertiesFile(var2.value);
                    if (var4 != null && var4.exists())
                    {
                        try
                        {
                            var3 = (PersistentState)var1.getConstructor([c]).newInstance([var2]);

                        }
                        catch (java.lang.Exception var7)
                        {
                            throw new RuntimeException("Failed to instantiate " + var1.toString(), var7);
                        }

                        FileInputStream var5 = new(var4);
                        NBTTagCompound var6 = NbtIo.Read(var5);
                        var5.close();
                        var3.readNBT(var6.GetCompoundTag("data"));
                    }
                }
                catch (java.lang.Exception var8)
                {
                    var8.printStackTrace();
                }
            }

            if (var3 != null)
            {
                loadedDataMap.put(var2, var3);
                loadedDataList.add(var3);
            }

            return var3;
        }
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
                java.io.File var2 = saveHandler.getWorldPropertiesFile(var1.id);
                if (var2 != null)
                {
                    NBTTagCompound var3 = new();
                    var1.writeNBT(var3);
                    NBTTagCompound var4 = new();
                    var4.SetCompoundTag("data", var3);

                    var saveTask = Task.Run(() =>
                    {
                        FileOutputStream var5 = new(var2);
                        NbtIo.WriteCompressed(var4, var5);
                        var5.close();
                    });

                    AsyncIO.addTask(saveTask);
                }
            }
            catch (System.Exception var6)
            {
                System.Console.WriteLine(var6);
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

            java.io.File var1 = saveHandler.getWorldPropertiesFile("idcounts");
            if (var1 != null && var1.exists())
            {
                DataInputStream var2 = new(new FileInputStream(var1));
                NBTTagCompound var3 = NbtIo.Read((DataInput)var2);
                var2.close();

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
        catch (java.lang.Exception var9)
        {
            var9.printStackTrace();
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
                java.io.File var3 = saveHandler.getWorldPropertiesFile("idcounts");
                if (var3 != null)
                {
                    NBTTagCompound var4 = new();
                    Iterator var5 = idCounts.keySet().iterator();

                    while (var5.hasNext())
                    {
                        string var6 = (string)var5.next();
                        short var7 = ((Short)idCounts.get(var6)).shortValue();
                        var4.SetShort(var6, var7);
                    }

                    DataOutputStream var9 = new(new FileOutputStream(var3));
                    NbtIo.Write(var4, var9);
                    var9.close();
                }
            }
            catch (java.lang.Exception var8)
            {
                var8.printStackTrace();
            }

            return var2.shortValue();
        }
    }
}