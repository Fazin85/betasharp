using BetaSharp.Entities;
using java.lang;

namespace BetaSharp;

public interface Monster : SpawnableEntity
{
    public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(Monster).TypeHandle);
}