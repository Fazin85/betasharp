namespace Beta3D;

public interface IMappedView : IDisposable
{
    Span<byte> Data { get; }
}
