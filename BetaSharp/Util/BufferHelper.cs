using System.Reflection;
using java.nio;

namespace BetaSharp.Util;

public static class BufferHelper
{
    // Cache reflection for performance
    private static readonly FieldInfo AddressField = typeof(java.nio.Buffer).GetField("address",
        BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// Executes an action with a native pointer to the buffer's current position.
    /// Handles both Direct and Heap buffers.
    /// </summary>
    public static unsafe void UsePointer(ByteBuffer buffer, Action<IntPtr> action)
    {
        if (buffer.isDirect())
        {
            long baseAddress = (long)AddressField.GetValue(buffer)!;
            IntPtr ptr = new(baseAddress);
            action(ptr);
        }
        else if (buffer.hasArray())
        {
            fixed (byte* p = buffer.array())
            {
                action((IntPtr)p);
            }
        }
        else
        {
            throw new NotSupportedException("Buffer type not supported.");
        }
    }
}