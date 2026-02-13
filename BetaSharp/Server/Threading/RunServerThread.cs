namespace BetaSharp.Server.Threading;

public class RunServerThread : java.lang.Thread
{
    private readonly MinecraftServer mcServer;

    public RunServerThread(MinecraftServer server, string name) : base(name)
    {
        mcServer = server;
    }

    public override void run()
    {
        mcServer.run();
    }
}