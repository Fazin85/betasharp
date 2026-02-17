namespace BetaSharp.Worlds.Chunks.Storage;

internal sealed class RegionFileChunkBuffer(RegionFile region, int var2, int var3) : MemoryStream(8096)
{
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (!disposing)
            {
                return;
            }
            
            var buffer = ToArray();
            region.write(var2, var3, buffer, buffer.Length);
        }
        finally
        {
            base.Dispose(disposing);
        }
    }
}