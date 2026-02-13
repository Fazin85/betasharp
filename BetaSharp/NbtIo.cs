using BetaSharp.NBT;
using java.io;
using java.util.zip;
using Console = System.Console;

namespace BetaSharp;

public static class NbtIo
{
    public static void Write(NBTTagCompound tag, DataOutput output)
    {
        NBTBase.WriteTag(tag, output);
    }

    public static void WriteCompressed(NBTTagCompound tag, OutputStream output)
    {
        var stream = new DataOutputStream(new GZIPOutputStream(output));

        try
        {
            Write(tag, stream);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to write a compressed NBT; {exception.Message}");
        }
        finally
        {
            stream.close();
        }
    }

    public static NBTTagCompound Read(InputStream input)
    {
        var stream = new DataInputStream(new GZIPInputStream(input));

        NBTTagCompound tag;

        try
        {
            tag = Read((DataInput) stream);
        }
        finally
        {
            stream.close();
        }

        return tag;
    }

    public static NBTTagCompound Read(DataInput input)
    {
        var tag = NBTBase.ReadTag(input);

        if (tag is NBTTagCompound compound)
        {
            return compound;
        }

        throw new InvalidOperationException("Root tag must be a named compound tag");
    }
}