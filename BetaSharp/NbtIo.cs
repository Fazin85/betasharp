using System.IO.Compression;
using BetaSharp.NBT;

namespace BetaSharp;

public static class NbtIo
{
    public static void Write(NBTTagCompound tag, Stream output)
    {
        NBTBase.WriteTag(tag, output);
    }

    public static void WriteCompressed(NBTTagCompound tag, Stream output)
    {
        using var compressor = new GZipStream(output, CompressionMode.Compress);
        Write(tag, compressor);
    }

    public static NBTTagCompound ReadCompressed(Stream input)
    {
        using var stream = new GZipStream(input, CompressionMode.Decompress);
        return Read(stream);
    }

    public static NBTTagCompound Read(Stream input)
    {
        if (NBTBase.ReadTag(input) is NBTTagCompound compound)
        {
            return compound;
        }

        throw new InvalidOperationException("Root tag must be a named compound tag");
    }
}