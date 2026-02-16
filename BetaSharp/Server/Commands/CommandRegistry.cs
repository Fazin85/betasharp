namespace BetaSharp.Server.Commands;

public static class CommandRegistry
{
    /// <summary>
    /// Gets the list of all available command names (first alias for each command)
    /// </summary>
    public static List<string> GetAvailableCommands()
    {
        return new List<string>
        {
            "kill",
            "heal",
            "clear",
            "tp",
            "teleport",
            "give",
            "time",
            "settime",
            "weather",
            "summon",
            "spawn",
            "killall",
            "say",
            "tell",
            "list",
            "stop",
            "save-all",
            "op",
            "deop",
            "ban",
            "pardon",
            "ban-ip",
            "pardon-ip",
            "kick",
            "whitelist",
            "save-off",
            "save-on",
            "help",
            "?"
        };
    }
}
