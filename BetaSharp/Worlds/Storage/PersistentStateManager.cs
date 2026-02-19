using System;
using System.Collections.Generic;
using System.IO;
using BetaSharp.NBT;

namespace BetaSharp.Worlds.Storage;

public class PersistentStateManager
{
    private readonly IWorldStorage _saveHandler;
    private readonly Dictionary<string, PersistentState> _loadedDataMap = new();
    private readonly List<PersistentState> _loadedDataList = new();
    private readonly Dictionary<string, short> _idCounts = new();

    public PersistentStateManager(IWorldStorage saveHandler)
    {
        _saveHandler = saveHandler;
        LoadIdCounts();
    }

    public T LoadData<T>(string name) where T : PersistentState
    {
        if (_loadedDataMap.TryGetValue(name, out PersistentState existing))
        {
            return (T)existing;
        }

        T state = null;

        if (_saveHandler != null)
        {
            try
            {
                string filePath = _saveHandler.GetWorldPropertiesFile(name);
                if (filePath != null && File.Exists(filePath))
                {
                    try
                    {
                        state = (T)Activator.CreateInstance(typeof(T), name);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to instantiate {typeof(T)}", e);
                    }

                    using var stream = File.OpenRead(filePath);
                    NBTTagCompound root = NbtIo.ReadCompressed(stream);
                    state.readNBT(root.GetCompoundTag("data"));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading persistent data '{name}': {ex.Message}");
            }
        }

        if (state != null)
        {
            _loadedDataMap[name] = state;
            _loadedDataList.Add(state);
        }

        return state;
    }

    public void SetData(string name, PersistentState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state), "Can't set null data");

        if (_loadedDataMap.ContainsKey(name))
        {
            _loadedDataList.Remove(_loadedDataMap[name]);
        }

        _loadedDataMap[name] = state;
        _loadedDataList.Add(state);
    }

    public void SaveAllData()
    {
        foreach (var state in _loadedDataList)
        {
            if (state.isDirty())
            {
                SaveData(state);
                state.setDirty(false);
            }
        }
    }

    private void SaveData(PersistentState state)
    {
        if (_saveHandler == null) return;

        try
        {
            string filePath = _saveHandler.GetWorldPropertiesFile(state.id);
            if (filePath != null)
            {
                NBTTagCompound dataTag = new();
                state.writeNBT(dataTag);

                NBTTagCompound root = new();
                root.SetCompoundTag("data", dataTag);

                using var stream = File.Create(filePath);
                NbtIo.WriteCompressed(root, stream);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to save persistent state {state.id}: {ex.Message}");
        }
    }

    private void LoadIdCounts()
    {
        try
        {
            _idCounts.Clear();
            if (_saveHandler == null) return;

            string filePath = _saveHandler.GetWorldPropertiesFile("idcounts");
            if (filePath != null && File.Exists(filePath))
            {
                using var stream = File.OpenRead(filePath);
                NBTTagCompound tag = NbtIo.Read(stream);

                foreach (var entry in tag.Values)
                {
                    if (entry is NBTTagShort shortTag)
                    {
                        _idCounts[shortTag.Key] = shortTag.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load idcounts: {ex.Message}");
        }
    }

    public int GetUniqueDataId(string key)
    {
        if (!_idCounts.TryGetValue(key, out short val))
        {
            val = 0;
        }
        else
        {
            val++;
        }

        _idCounts[key] = val;

        if (_saveHandler != null)
        {
            try
            {
                string filePath = _saveHandler.GetWorldPropertiesFile("idcounts");
                if (filePath != null)
                {
                    NBTTagCompound tag = new();
                    foreach (var kvp in _idCounts)
                    {
                        tag.SetShort(kvp.Key, kvp.Value);
                    }

                    using var stream = File.Create(filePath);
                    NbtIo.Write(tag, stream);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save unique ID counts: {ex.Message}");
            }
        }

        return val;
    }
}
