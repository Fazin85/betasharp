namespace Beta3D.FrameGraph;

public interface IFramePass
{
    IResourceHandle<T> CreatesInternal<T>(string name, IResourceDescriptor<T> descriptor);

    void Reads<T>(IResourceHandle<T> handle);

    IResourceHandle<T> ReadsAndWrites<T>(IResourceHandle<T> handle);

    void Requires(IFramePass pass);

    void DisableCulling();

    void Executes(Action task);
}
