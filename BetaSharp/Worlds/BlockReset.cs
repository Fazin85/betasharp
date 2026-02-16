namespace BetaSharp.Worlds;

public class BlockReset
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Delay { get; set; }
    public int BlockId { get; set; }
    public int Meta { get; set; }
    
    public ClientWorld World { get; }

    public BlockReset(ClientWorld world, int x, int y, int z, int blockId, int meta)
    {
        World = world;
        X = x;
        Y = y;
        Z = z;
        BlockId = blockId;
        Meta = meta;
        
        Delay = 80; 
    }
}