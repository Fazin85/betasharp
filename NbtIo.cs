using betareborn.NBT;
using java.io;
using java.util.zip;
using Console = System.Console;

namespace betareborn
{
    public static class NbtIo
    {
        public static void Write(NbtTagCompound tag, DataOutput output)
        {
            NBTBase.WriteTag(tag, output);
        }

        public static void WriteCompressed(NbtTagCompound tag, OutputStream output)
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

        public static NbtTagCompound Read(InputStream input)
        {
            var stream = new DataInputStream(new GZIPInputStream(input));

            NbtTagCompound tag;

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

        public static NbtTagCompound Read(DataInput input)
        {
            var tag = NBTBase.ReadTag(input);

            if (tag is NbtTagCompound compound)
            {
                return compound;
            }

            throw new InvalidOperationException("Root tag must be a named compound tag");
        }
    }
}