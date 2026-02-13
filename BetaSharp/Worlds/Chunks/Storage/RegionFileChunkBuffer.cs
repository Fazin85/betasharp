using java.io;

namespace BetaSharp.Worlds.Chunks.Storage;

public class RegionFileChunkBuffer : ByteArrayOutputStream
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

    public override void close()
    {
        regionFile.write(chunkX, chunkZ, buf, count);
    }
}