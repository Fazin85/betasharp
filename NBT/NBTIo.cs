using java.io;
using java.util.zip;

namespace betareborn.NBT
{
    public class NBTIo : java.lang.Object
    {
        public static NBTTagCompound read(InputStream input)
        {
            var stream = new DataInputStream(new GZIPInputStream(input));

            NBTTagCompound tag;

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

        public static void writeGzippedCompoundToOutputStream(NBTTagCompound tag, OutputStream output)
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

        public static NBTTagCompound read(DataInput input)
        {
            var tag = NBTBase.readTag(input);
            
            if (tag is NBTTagCompound compound)
            {
                return compound;
            }

            throw new InvalidOperationException("Root tag must be a named compound tag");
        }

        public static void write(NBTTagCompound tag, DataOutput output)
        {
            NBTBase.writeTag(tag, output);
        }
    }
}