namespace BetaSharp;

public class WatchableObject(int objectType, int dataValueId, object watchedObject)
{
    public readonly int objectType = objectType;
    public readonly int dataValueId = dataValueId;
    public object watchedObject = watchedObject;
    public bool dirty = true;
}