using java.io;

namespace BetaSharp.Worlds.Chunks.Storage;

public class RegionFileChunkBuffer : ByteArrayOutputStream
{
    private readonly int chunkX;
    private readonly int chunkZ;
    private readonly RegionFile regionFile;

    public RegionFileChunkBuffer(RegionFile regionFile, int chunkX, int chunkZ) : base(8096)
    {
        this.regionFile = regionFile;
        this.chunkX = chunkX;
        this.chunkZ = chunkZ;
    }

    public override void close()
    {
        regionFile.write(chunkX, chunkZ, buf, count);
    }
}