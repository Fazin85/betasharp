namespace betareborn
{
    public class WatchableObject
    {
        public readonly int objectType;
        public readonly int dataValueId;
        public java.lang.Object watchedObject;
        public bool dirty;

        public WatchableObject(int objectType, int dataValueId, java.lang.Object watchedObject)
        {
            this.dataValueId = dataValueId;
            this.watchedObject = watchedObject;
            this.objectType = objectType;
            dirty = true;
        }
    }
}