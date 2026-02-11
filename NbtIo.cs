using betareborn.NBT;
using java.io;
using java.util.zip;

namespace betareborn
{
    public class NbtIo : java.lang.Object
    {
        public static NbtTagCompound read(InputStream input)
        {
            var stream = new DataInputStream(new GZIPInputStream(input));

            NbtTagCompound tag;

            try
            {
                tag = read((DataInput) stream);
            }
            finally
            {
                stream.close();
            }

            return tag;
        }

        public static void writeGzippedCompoundToOutputStream(NbtTagCompound tag, OutputStream output)
        {
            var stream = new DataOutputStream(new GZIPOutputStream(output));

            try
            {
                write(tag, stream);
            }
            finally
            {
                stream.close();
            }
        }

        public static NbtTagCompound read(DataInput input)
        {
            var tag = NBTBase.ReadTag(input);
            
            if (tag is NbtTagCompound compound)
            {
                return compound;
            }

            throw new InvalidOperationException("Root tag must be a named compound tag");
        }

        public static void write(NbtTagCompound tag, DataOutput output)
        {
            NBTBase.WriteTag(tag, output);
        }
    }
}