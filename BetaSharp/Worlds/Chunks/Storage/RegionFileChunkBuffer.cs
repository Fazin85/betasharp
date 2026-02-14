namespace BetaSharp.Worlds.Chunks.Storage;

public class RegionFileChunkBuffer : MemoryStream
{
    private readonly int chunkX;
    private readonly int chunkZ;
    private readonly RegionFile regionFile;

    public RegionFileChunkBuffer(RegionFile var1, int var2, int var3) : base(8096)
    {
        regionFile = var1;
        chunkX = var2;
        chunkZ = var3;
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                byte[] buffer = ToArray();
                regionFile.write(chunkX, chunkZ, buffer, buffer.Length);
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }
}