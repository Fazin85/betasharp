namespace BetaSharp.Server.Commands;

internal class Command
{
    public readonly string commandAndArgs;
    public readonly CommandOutput output;

    public Command(string contents, CommandOutput output)
    {
        commandAndArgs = contents;
        this.output = output;
    }
}
