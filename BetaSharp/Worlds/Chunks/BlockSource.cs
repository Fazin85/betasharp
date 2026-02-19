using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Chunks;

public class BlockSource
{
    private static byte[] BLOCKS = new byte[256];

    public static void fill(byte[] blocks)
    {
        for (int var1 = 0; var1 < blocks.Length; ++var1)
        {
            blocks[var1] = BLOCKS[blocks[var1] & 255];
        }

    }

    static BlockSource()
    {
        try
        {
            for (int var0 = 0; var0 < 256; ++var0)
            {
                byte var1 = (byte)var0;
                if (var1 != 0 && Block.Blocks[var1 & 255] == null)
                {
                    var1 = 0;
                }

                BLOCKS[var0] = var1;
            }
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }

    }
}