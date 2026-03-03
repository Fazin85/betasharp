namespace BetaSharp.Util;

internal static class BufferHelper
{
    /// <summary>
    /// Executes an action with a native pointer to a byte array's data.
    /// </summary>
    public static unsafe void UsePointer(byte[] buffer, Action<IntPtr> action)
    {
        fixed (byte* p = buffer)
        {
            action((IntPtr)p);
        }
    }
}
