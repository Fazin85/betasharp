using java.lang;
using java.nio;
using java.util;

namespace BetaSharp.Client.Rendering.Core;

public class GLAllocation
{
    private static readonly List displayLists = new ArrayList();
    private static readonly List textureNames = new ArrayList();
    private static readonly object l = new();
    public static int generateDisplayLists(int var0)
    {
        lock (l)
        {
            int var1 = (int)GLManager.GL.GenLists((uint)var0);
            displayLists.add(Integer.valueOf(var1));
            displayLists.add(Integer.valueOf(var0));
            return var1;
        }
    }

    public static void generateTextureNames(IntBuffer var0)
    {
        lock (l)
        {
            uint[] textureIds = new uint[var0.remaining()];
            GLManager.GL.GenTextures(textureIds);

            int[] intIds = Array.ConvertAll(textureIds, id => (int)id);
            var0.put(intIds);

            for (int var1 = var0.position() - intIds.Length; var1 < var0.position(); ++var1)
            {
                textureNames.add(Integer.valueOf(var0.get(var1)));
            }
        }
    }

    public static void generateBuffersARB(IntBuffer vertexBuffers)
    {
        lock (l)
        {
            uint[] bufferIds = new uint[vertexBuffers.remaining()];
            GLManager.GL.GenBuffers(bufferIds);
            int[] intIds = Array.ConvertAll(bufferIds, id => (int)id);
            vertexBuffers.put(intIds);
        }
    }

    public static void func_28194_b(int var0)
    {
        lock (l)
        {
            int var1 = displayLists.indexOf(Integer.valueOf(var0));
            int list = ((Integer)displayLists.get(var1)).intValue();
            int range = ((Integer)displayLists.get(var1 + 1)).intValue();
            GLManager.GL.DeleteLists((uint)list, (uint)range);
            displayLists.remove(var1);
            displayLists.remove(var1);
        }
    }

    public static void deleteTexturesAndDisplayLists()
    {
        lock (l)
        {
            for (int var0 = 0; var0 < displayLists.size(); var0 += 2)
            {
                int list = ((Integer)displayLists.get(var0)).intValue();
                int range = ((Integer)displayLists.get(var0 + 1)).intValue();
                GLManager.GL.DeleteLists((uint)list, (uint)range);
            }

            if (textureNames.size() > 0)
            {
                uint[] textureIds = new uint[textureNames.size()];
                for (int i = 0; i < textureNames.size(); i++)
                {
                    textureIds[i] = (uint)((Integer)textureNames.get(i)).intValue();
                }
                GLManager.GL.DeleteTextures(textureIds);
            }

            displayLists.clear();
            textureNames.clear();
        }
    }

    public static ByteBuffer createDirectByteBuffer(int capacity)
    {
        lock (l)
        {
            ByteBuffer var1 = ByteBuffer.allocateDirect(capacity).order(ByteOrder.nativeOrder());
            return var1;
        }
    }

    public static IntBuffer createDirectIntBuffer(int var0)
    {
        return createDirectByteBuffer(var0 << 2).asIntBuffer();
    }

    public static FloatBuffer createDirectFloatBuffer(int var0)
    {
        return createDirectByteBuffer(var0 << 2).asFloatBuffer();
    }
}
