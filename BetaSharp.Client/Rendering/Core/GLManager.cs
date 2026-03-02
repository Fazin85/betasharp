using Vuldrid;

namespace BetaSharp.Client.Rendering.Core;

/// <summary>
/// Central manager for Vuldrid graphics resources.
/// Provides the GraphicsDevice, CommandList, and the emulated GL interface.
/// </summary>
public class GLManager
{
    public static IGL GL { get; private set; }
    public static GraphicsDevice Device { get; private set; }
    public static ResourceFactory Factory { get; private set; }
    public static CommandList CommandList { get; private set; }
    public static Framebuffer SwapchainFramebuffer => Device.MainSwapchain.Framebuffer;

    public static void Init(GraphicsDevice device)
    {
        Device = device;
        Factory = device.ResourceFactory;
        CommandList = Factory.CreateCommandList();
        GL = new OpenGL.EmulatedGL(device);
    }

    /// <summary>
    /// Begin recording commands for a new frame.
    /// </summary>
    public static void BeginFrame()
    {
        CommandList.Begin();
        CommandList.SetFramebuffer(SwapchainFramebuffer);
    }

    /// <summary>
    /// End recording and submit commands, then present.
    /// </summary>
    public static void EndFrame()
    {
        CommandList.End();
        Device.SubmitCommands(CommandList);
    }

    public static void Dispose()
    {
        CommandList?.Dispose();
    }
}
