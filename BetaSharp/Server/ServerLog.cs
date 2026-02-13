using java.util.logging;

namespace BetaSharp.Server;

public class ServerLog
{
    public static Logger LOGGER = Logger.getLogger("Minecraft");
    private static bool initialized = false;

    public static void init()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        ConsoleFormatter var0 = new ConsoleFormatter();
        LOGGER.setUseParentHandlers(false);
        ConsoleHandler var1 = new ConsoleHandler();
        var1.setFormatter(var0);
        LOGGER.addHandler(var1);

        try
        {
            FileHandler var2 = new FileHandler("server.log", true);
            var2.setFormatter(var0);
            LOGGER.addHandler(var2);
        }
        catch (java.lang.Exception var3)
        {
            LOGGER.log(Level.WARNING, "Failed to log to server.log", var3);
        }
    }
}