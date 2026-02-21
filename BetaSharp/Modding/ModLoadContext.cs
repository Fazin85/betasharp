using System.Reflection;
using System.Runtime.Loader;

namespace BetaSharp.Modding;

public class ModLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public ModLoadContext(string modPath)
        : base(isCollectible: true)
    {
        _resolver = new(modPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path != null ? LoadFromAssemblyPath(path) : null;
    }
}
