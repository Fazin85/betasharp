using betareborn.Entities;
using java.lang;

namespace betareborn
{
    public interface Monster : SpawnableEntity
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(Monster).TypeHandle);
    }
}