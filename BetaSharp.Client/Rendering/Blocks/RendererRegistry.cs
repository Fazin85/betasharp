namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class RendererRegistry
{
    private readonly IBlockRenderer[] _meshers = new IBlockRenderer[32];

    public IBlockRenderer this[RendererType i]
    {

        get
        {
            int index = (int)i;
            if (index >= 0 && index < _meshers.Length)
            {
                return _meshers[index];
            }
            throw new IndexOutOfRangeException("Index is outside the bounds of the array.");
        }
        set
        {
            int index = (int)i;
            if (index >= 0 && index < _meshers.Length)
            {
                _meshers[index] = value;
            }
            else
            {
                throw new IndexOutOfRangeException("Index is outside the bounds of the array.");
            }
        }
    }
}
