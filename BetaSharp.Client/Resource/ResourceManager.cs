namespace BetaSharp.Client.Resource;

public class ResourceManager : IDisposable
{
    private readonly List<IResourceLoader> _loaders = [];

    public ResourceManager Add(IResourceLoader loader)
    {
        _loaders.Add(loader);
        return this;
    }

    public async Task LoadAllAsync()
    {
        foreach (IResourceLoader loader in _loaders)
        {
            await loader.LoadAsync();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (IResourceLoader loader in _loaders)
        {
            if (loader is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
