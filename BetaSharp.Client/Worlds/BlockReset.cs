namespace BetaSharp.Client.Worlds;

public class BlockReset(ClientWorld world, int x, int y, int z, int blockId, int meta)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Z { get; set; } = z;
    public int Delay { get; set; } = 80;
    public int BlockId { get; set; } = blockId;
    public int Meta { get; set; } = meta;

    public ClientWorld World { get; } = world;
}
