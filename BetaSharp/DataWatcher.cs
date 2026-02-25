using BetaSharp.Items;
using BetaSharp.Network.Packets;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp;

public class DataWatcher
{
    private static readonly Dictionary<Type, int> dataTypes = [];
        
    private readonly Dictionary<int, WatchableObject> watchedObjects = new();
    public bool dirty { get; private set; }
    
    public void AddObject(int id, object value)
    {
        if (!dataTypes.TryGetValue(value.GetType(), out int typeId)) 
        {
            throw new ArgumentException("Unknown data type: " + value.GetType());
        }

        if (id > 31)
        {
            throw new ArgumentException("Data value id is too big with " + id + "! (Max is " + 31 + ")");
        }

        if (watchedObjects.ContainsKey(id))
        {
            throw new ArgumentException("Duplicate id value for " + id + "!");
        }

        watchedObjects[id] = new WatchableObject(typeId, id, value);
    }

    public List<WatchableObject> GetDirtyEntries()
    {
	    List<WatchableObject> res = null;
        if (dirty)
        {
            foreach (var obj in watchedObjects.Values)
            {
                if (obj.dirty)
                {
                    if (res == null) res = new List<WatchableObject>();
                        
                    obj.dirty = false;
                    res.Add(obj);
                }
            }
        }

        dirty = false;
        return res;
    }

    public sbyte getWatchableObjectByte(int id)
    {
        return (sbyte)((byte)watchedObjects[id].watchedObject);
    }

    public int GetWatchableObjectInt(int id)
    {
        return (int)watchedObjects[id].watchedObject;
    }
        
    public string GetWatchableObjectString(int id)
    {
        return ((string)watchedObjects[id].watchedObject);
    }

    public void UpdateObject(int id, object value)
    {
        WatchableObject obj = watchedObjects[id];
        if (!value.Equals(obj.watchedObject))
        {
            obj.watchedObject = value;
            obj.dirty = true;
            dirty = true;
        }
    }

    public static void WriteObjectsInListToStream(List<WatchableObject> list, DataOutputStream stream)
    {
        if (list != null)
        {
	        foreach (WatchableObject o in list)
	        {
		        WriteWatchableObject(stream, o);
	        }
        }

        stream.writeByte(127);
    }

    public void WriteWatchableObjects(DataOutputStream stream)
    {
        foreach (var obj in watchedObjects.Values)
        {
            WriteWatchableObject(stream, obj);
        }

        stream.writeByte(127);
    }

    private static void WriteWatchableObject(DataOutputStream stream, WatchableObject obj)
    {
        int header = (obj.objectType << 5 | obj.dataValueId & 31) & 255;
        stream.writeByte(header);
        switch (obj.objectType)
        {
            case 0:
                stream.writeByte((byte)(obj.watchedObject));
                break;
            case 1:
                stream.writeShort((short)obj.watchedObject);
                break;
            case 2:
                stream.writeInt((int)obj.watchedObject);
                break;
            case 3:
                stream.writeFloat((float)obj.watchedObject);
                break;
            case 4:
                Packet.WriteString((string)obj.watchedObject, stream);
                break;
            case 5:
                ItemStack item = (ItemStack)obj.watchedObject;
                stream.writeShort(item.getItem().id);
                stream.writeByte(item.count);
                stream.writeShort(item.getDamage());
                break;
            case 6:
                Vec3i vec = (Vec3i)obj.watchedObject;
                stream.writeInt(vec.X);
                stream.writeInt(vec.Y);
                stream.writeInt(vec.Z);
                break;
        }
    }

    public static List<WatchableObject> ReadWatchableObjects(DataInputStream stream)
    {
	    List<WatchableObject> res = null;

        for (sbyte b = (sbyte)stream.readByte(); b != 127; b = (sbyte)stream.readByte())
        {
            res ??= [];

            int objectType = (b & 224) >> 5;
            int dataValueId = b & 31;
            WatchableObject obj = null;
            switch (objectType)
            {
                case 0:
                    obj = new WatchableObject(objectType, dataValueId, stream.readByte());
                    break;
                case 1:
                    obj = new WatchableObject(objectType, dataValueId, stream.readShort());
                    break;
                case 2:
                    obj = new WatchableObject(objectType, dataValueId, stream.readInt());
                    break;
                case 3:
                    obj = new WatchableObject(objectType, dataValueId, stream.readFloat());
                    break;
                case 4:
                    obj = new WatchableObject(objectType, dataValueId, Packet.ReadString(stream, 64));
                    break;
                case 5:
                    short id = stream.readShort();
                    sbyte count = (sbyte)stream.readByte();
                    short damage = stream.readShort();
                    obj = new WatchableObject(objectType, dataValueId, new ItemStack(id, count, damage));
                    break;
                case 6:
                    int x = stream.readInt();
                    int y = stream.readInt();
                    int z = stream.readInt();
                    obj = new WatchableObject(objectType, dataValueId, new Vec3i(x, y, z));
                    break;
            }

            res.Add(obj);
        }

        return res;
    }

    public void UpdateWatchedObjectsFromList(List<WatchableObject> list)
    {
	    foreach (WatchableObject obj in list)
	    {
		    if (watchedObjects.TryGetValue(obj.dataValueId, out var obj2))
		    {
			    obj2.watchedObject = obj.watchedObject;
		    }
	    }
    }

    static DataWatcher()
    {
        dataTypes[typeof(byte)] =  0;
        dataTypes[typeof(short)] =  1;
        dataTypes[typeof(int)] =  2;
        dataTypes[typeof(float)] =  3;
        dataTypes[typeof(string)] =  4;
        dataTypes[typeof(ItemStack)] =  5;
        dataTypes[typeof(Vec3i)] =  6;
    }
}